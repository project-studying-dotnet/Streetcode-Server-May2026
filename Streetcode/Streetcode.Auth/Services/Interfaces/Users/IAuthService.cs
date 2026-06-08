using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Models.Entities;

namespace Streetcode.Auth.Services.Interfaces.Users
{
    public interface IAuthService
    {
        Task<LoginResultDto> CreateLoginResultAsync(User user);
    }
}