using Google.Apis.Auth;

namespace Streetcode.Auth.Services.Interfaces.Users
{
    public interface IGoogleAuthService
    {
        Task<GoogleJsonWebSignature.Payload> ValidateTokenAsync(string idToken);
    }
}
