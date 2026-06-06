namespace Streetcode.Auth.Settings
{
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;

        public string Issuer { get; set; } = string.Empty;

        public string Audience { get; set; } = string.Empty;

        public int AccessTokenLifetimeInMinutes { get; set; } = 120;

        public int RefreshTokenLifetimeInDays { get; set; } = 7;
    }
}
