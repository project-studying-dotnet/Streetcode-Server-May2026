namespace Streetcode.Auth.Models.DTO
{
    public class RefreshTokenResponce
    {
        required public string Token { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
