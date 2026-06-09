namespace Streetcode.Auth.Models.DTO
{
    public class AuthResponseDto
    {
        required public UserDto User { get; set; }
        required public string Token { get; set; }
        required public string RefreshToken { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
