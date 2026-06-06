using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Streetcode.Auth.Data;
using Streetcode.Auth.Models.Entities;
using Streetcode.Auth.Services.Interfaces.Users;
using Streetcode.Auth.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Streetcode.Auth.Services.Users
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public TokenService(JwtSettings jwtSettings, UserManager<User> userManager, ApplicationDbContext context)
        {
            _jwtSettings = jwtSettings;
            _userManager = userManager;
            _context = context; 
        }

        public JwtSecurityToken GenerateJWTToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

            return CreateToken(claims);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(
                token,
                GetTokenValidationParameters(validateLifetime: false),
                out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token.");
            }

            return principal;
        }
        public async Task<(JwtSecurityToken Jwt, string NewRefreshToken)> RefreshTokenAsync(string token)
        {
            var tokenHash = ComputeHash(token);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var storedToken = await _context.RefreshTokens
                    .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);

                if (storedToken == null || storedToken.IsRevoked || storedToken.IsUsed)
                    throw new SecurityTokenException("Invalid refresh token.");

                if (storedToken.Expires < DateTime.UtcNow)
                {
                    storedToken.IsRevoked = true;
                    await _context.SaveChangesAsync();
                    throw new SecurityTokenException("Refresh token expired.");
                }

                storedToken.IsUsed = true;
                storedToken.IsRevoked = true;

                var user = await _userManager.FindByIdAsync(storedToken.UserId.ToString())
                           ?? throw new SecurityTokenException("User not found.");

                var newJwt = GenerateJWTToken(user);
                var newRefreshToken = GenerateRefreshToken();

                await SaveRefreshTokenAsync(user.Id, newRefreshToken);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (newJwt, newRefreshToken);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task RevokeTokenAsync(string token)
        {
            var tokenHash = ComputeHash(token);
            var storedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);

            if (storedToken != null)
            {
                storedToken.IsRevoked = true;
                await _context.SaveChangesAsync();
            }
        }
        public async Task SaveRefreshTokenAsync(int userId, string token)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                throw new Exception($"Пользователь с ID {userId} не найден в базе данных!");
            }
            var refreshToken = new RefreshToken
            {
                TokenHash = ComputeHash(token),
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeInDays),
                Created = DateTime.UtcNow,
                UserId = userId
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }

        private string ComputeHash(string input)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }

        private JwtSecurityToken CreateToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenLifetimeInMinutes),
                signingCredentials: credentials);
        }

        private TokenValidationParameters GetTokenValidationParameters(bool validateLifetime)
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = validateLifetime,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
            };
        }
    }
}