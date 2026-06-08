//using AutoMapper;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Streetcode.Auth.Data;
//using Streetcode.Auth.Extensions;
//using Streetcode.Auth.Models.DTO;
//using Streetcode.Auth.Models.Entities;
//using Streetcode.Auth.Services.Interfaces.Users;
//using Streetcode.Common.Configuration;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Security.Cryptography;
//using System.Text;

//namespace Streetcode.Auth.Services.Users
//{
//    public class TokenService : IJwtTokenService
//    {
//        private readonly JwtSettings _jwtSettings;
//        private readonly UserManager<User> _userManager;
//        private readonly ApplicationDbContext _context;
//        private readonly IMapper _mapper;

//        public TokenService(
//                JwtSettings jwtSettings,
//                UserManager<User> userManager,
//                ApplicationDbContext context,
//                IMapper mapper)
//        {
//            _jwtSettings = jwtSettings;
//            _userManager = userManager;
//            _context = context;
//            _mapper = mapper;
//        }

//        public JwtSecurityToken GenerateJWTToken(User user)
//        {
//            var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
//                new Claim(ClaimTypes.Role, user.Role.ToString()),
//            };

//            return CreateToken(claims);
//        }

//        public string GenerateRefreshToken()
//        {
//            var randomBytes = new byte[64];
//            using var randomNumberGenerator = RandomNumberGenerator.Create();
//            randomNumberGenerator.GetBytes(randomBytes);
//            return Convert.ToBase64String(randomBytes);
//        }

//        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
//        {
//            var tokenHandler = new JwtSecurityTokenHandler();

//            var principal = tokenHandler.ValidateToken(
//                token,
//                GetTokenValidationParameters(validateLifetime: false),
//                out SecurityToken securityToken);

//            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
//                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
//            {
//                throw new SecurityTokenException("Invalid token.");
//            }

//            return principal;
//        }
//        public async Task<(JwtSecurityToken Jwt, string NewRefreshToken)> RefreshTokenAsync(string token)
//        {
//            var tokenHash = ComputeHash(token);

//            using var transaction = await _context.Database.BeginTransactionAsync();

//            try
//            {
//                var storedToken = await _context.RefreshTokens
//                    .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);

//                if (storedToken == null || storedToken.IsRevoked || storedToken.IsUsed)
//                    throw new SecurityTokenException("Invalid refresh token.");

//                if (storedToken.Expires < DateTime.UtcNow)
//                {
//                    storedToken.IsRevoked = true;
//                    await _context.SaveChangesAsync();
//                    throw new SecurityTokenException("Refresh token expired.");
//                }

//                storedToken.IsUsed = true;
//                storedToken.IsRevoked = true;

//                var user = await _userManager.FindByIdAsync(storedToken.UserId.ToString())
//                           ?? throw new SecurityTokenException("User not found.");

//                var newJwt = GenerateJWTToken(user);
//                var newRefreshToken = GenerateRefreshToken();

//                await SaveRefreshTokenAsync(user.Id, newRefreshToken);

//                await _context.SaveChangesAsync();
//                await transaction.CommitAsync();

//                return (newJwt, newRefreshToken);
//            }
//            catch
//            {
//                await transaction.RollbackAsync();
//                throw;
//            }
//        }

//        public async Task RevokeTokenAsync(string token)
//        {
//            var tokenHash = ComputeHash(token);
//            var storedToken = await _context.RefreshTokens
//                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);

//            if (storedToken != null)
//            {
//                storedToken.IsRevoked = true;
//                await _context.SaveChangesAsync();
//            }
//        }

//        public async Task<RefreshResult> RefreshAsync(string refreshToken)
//        {
//            var tokenHash = Hash(refreshToken);

//            var storedToken = await _db.RefreshTokens
//                .Include(x => x.User)
//                .FirstOrDefaultAsync(x => x.TokenHash == tokenHash);

//            if (storedToken is null)
//                throw new SecurityTokenException("Token not found");

//            if (storedToken.IsRevoked || storedToken.IsUsed)
//                throw new SecurityTokenException("Token already used");

//            if (storedToken.Expires < DateTime.UtcNow)
//                throw new SecurityTokenException("Token expired");

//            // 🔥 ROTATION (старый токен помечаем)
//            storedToken.IsUsed = true;

//            // создаём новый refresh token
//            var newRefreshToken = GenerateRefreshToken();
//            var newHash = Hash(newRefreshToken);

//            storedToken.ReplacedByTokenHash = newHash;

//            _db.RefreshTokens.Add(new RefreshToken
//            {
//                TokenHash = newHash,
//                UserId = storedToken.UserId,
//                Created = DateTime.UtcNow,
//                Expires = DateTime.UtcNow.AddDays(7),
//                IsRevoked = false,
//                IsUsed = false
//            });

//            await _db.SaveChangesAsync();

//            // создаём JWT
//            var jwt = CreateJwtToken(storedToken.User);

//            return new RefreshResult
//            {
//                AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
//                RefreshToken = newRefreshToken,
//                UserId = storedToken.UserId.ToString(),
//                ExpiresAt = jwt.ValidTo
//            };
//        }
//        public async Task SaveRefreshTokenAsync(int userId, string token)
//        {
//            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
//            if (!userExists)
//            {
//                throw new Exception($"Пользователь с ID {userId} не найден в базе данных!");
//            }
//            var refreshToken = new RefreshToken
//            {
//                TokenHash = ComputeHash(token),
//                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeInDays),
//                Created = DateTime.UtcNow,
//                UserId = userId
//            };

//            _context.RefreshTokens.Add(refreshToken);
//            await _context.SaveChangesAsync();
//        }

//        private string ComputeHash(string input)
//        {
//            using var sha256 = System.Security.Cryptography.SHA256.Create();
//            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
//            return Convert.ToBase64String(bytes);
//        }

//        private JwtSecurityToken CreateToken(IEnumerable<Claim> claims)
//        {
//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
//            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            return new JwtSecurityToken(
//                issuer: _jwtSettings.Issuer,
//                audience: _jwtSettings.Audience,
//                claims: claims,
//                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenLifetimeInMinutes),
//                signingCredentials: credentials);
//        }

//        private TokenValidationParameters GetTokenValidationParameters(bool validateLifetime)
//        {
//            return new TokenValidationParameters
//            {
//                ValidateIssuer = true,
//                ValidateAudience = true,
//                ValidateLifetime = validateLifetime,
//                ValidateIssuerSigningKey = true,
//                ValidIssuer = _jwtSettings.Issuer,
//                ValidAudience = _jwtSettings.Audience,
//                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
//            };
//        }

//        public async Task<LoginResultDto> CreateLoginResultAsync(User user)
//        {
            
//            var jwtToken = GenerateJWTToken(user);
//            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
//            var refreshToken = GenerateRefreshToken();

//            await SaveRefreshTokenAsync(user.Id, refreshToken);

//            user.EnsureSecurityStamp();
//            await _userManager.UpdateAsync(user);

//            return new LoginResultDto
//            {
//                User = _mapper.Map<UserDto>(user),
//                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
//                RefreshToken = refreshToken,
//                ExpireAt = jwtToken.ValidTo
//            };
//        }
//    }
//}