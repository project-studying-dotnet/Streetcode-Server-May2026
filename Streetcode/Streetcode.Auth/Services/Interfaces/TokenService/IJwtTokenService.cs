using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Streetcode.Auth.Services.Interfaces.Users
{
    public interface IJwtTokenService
    {
        JwtSecurityToken GenerateToken(User user);
        
        //JwtSecurityToken GenerateJWTToken(User user);

        //string GenerateRefreshToken();

        //ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

        //Task<(JwtSecurityToken Jwt, string NewRefreshToken)> RefreshTokenAsync(string token);

        //Task SaveRefreshTokenAsync(int userId, string token);

        //Task RevokeTokenAsync(string token);

        //Task<RefreshResult> RefreshAsync(string refreshToken);

        //Task<AuthResponseDto> CreateLoginResultAsync(User user);
    }
}
