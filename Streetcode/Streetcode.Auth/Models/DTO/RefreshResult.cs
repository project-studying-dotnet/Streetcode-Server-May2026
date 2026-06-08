namespace Streetcode.Auth.Models.DTO
{
    public class RefreshResult
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }
}
