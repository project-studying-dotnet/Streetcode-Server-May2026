namespace Streetcode.Auth.Models.DTO
{
    public class RefreshTokenRequestDto
    {
        required public string Token { get; set; }

        required public string RefreshToken { get; set; }
    }
}