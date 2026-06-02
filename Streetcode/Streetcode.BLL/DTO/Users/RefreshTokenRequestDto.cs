namespace Streetcode.BLL.DTO.Users
{
    public class RefreshTokenRequestDto
    {
        required public string Token { get; set; }

        required public string RefreshToken { get; set; }
    }
}