using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Streetcode.DAL.Entities.Users;

namespace Streetcode.BLL.Interfaces.Users
{
    public interface ITokenService
    {
        JwtSecurityToken GenerateJWTToken(User user);

        string GenerateRefreshToken();

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

        JwtSecurityToken RefreshToken(string token);
    }
}
