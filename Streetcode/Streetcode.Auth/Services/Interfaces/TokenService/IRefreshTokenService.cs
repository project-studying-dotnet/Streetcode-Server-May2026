using Streetcode.Auth.Models.Entities;

namespace Streetcode.Auth.Services.Interfaces.Users
{
    public interface IRefreshTokenService
    {
        Task<(User user, string newRefreshToken)> RefreshAsync(string refreshToken);
        Task SaveAsync(int userId, string refreshToken);
        Task RevokeAsync(string refreshToken);
        string Generate();
    }
}