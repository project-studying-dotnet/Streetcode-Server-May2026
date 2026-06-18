using Google.Apis.Auth;

namespace Streetcode.WebApi.Service.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<GoogleJsonWebSignature.Payload> ValidateTokenAsync(string idToken);
    }
}
