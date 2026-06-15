using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Streetcode.Auth.Data;
using Streetcode.Auth.Models.Entities;
using Streetcode.Auth.Services.Interfaces.Users;
using System.Security.Cryptography;

namespace Streetcode.Auth.Services.Users
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ApplicationDbContext _context;

        public RefreshTokenService(ApplicationDbContext context)
        {
            _context = context;
        }

        public string Generate()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public async Task<(User user, string newRefreshToken)> RefreshAsync(string refreshToken)
        {
            var hash = ComputeHash(refreshToken);

            var stored = _context.RefreshTokens.Include(x => x.User).FirstOrDefault(x => x.TokenHash == hash);

            if (stored == null || stored.IsRevoked || stored.IsUsed)
                throw new SecurityTokenException("Invalid refresh token");

            if (stored.Expires < DateTime.UtcNow)
                throw new SecurityTokenException("Refresh token expired");

            stored.IsUsed = true;
            stored.IsRevoked = true;

            var newRefresh = Generate();
            var newHash = ComputeHash(newRefresh);

            _context.RefreshTokens.Add(new RefreshToken
            {
                TokenHash = newHash,
                UserId = stored.UserId,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7),
                IsUsed = false,
                IsRevoked = false
            });

            await _context.SaveChangesAsync();

            return (stored.User, newRefresh);
        }

        public async Task SaveAsync(int userId, string refreshToken)
        {
            var entity = new RefreshToken
            {
                TokenHash = ComputeHash(refreshToken),
                UserId = userId,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7),
                IsUsed = false,
                IsRevoked = false
            };

            _context.RefreshTokens.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task RevokeAsync(string refreshToken)
        {
            var hash = ComputeHash(refreshToken);

            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.TokenHash == hash);

            if (token != null)
            {
                token.IsRevoked = true;
                await _context.SaveChangesAsync();
            }
        }

        private static string ComputeHash(string input)
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            var hashBytes = SHA256.HashData(inputBytes);

            return Convert.ToBase64String(hashBytes);
        }
    }
}