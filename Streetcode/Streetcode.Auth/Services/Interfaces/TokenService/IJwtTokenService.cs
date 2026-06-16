using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Streetcode.Auth.Services.Interfaces.Users
{
    public interface IJwtTokenService
    {
        JwtSecurityToken GenerateToken(User user);
    }
}
