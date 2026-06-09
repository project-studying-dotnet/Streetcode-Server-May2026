namespace Streetcode.Auth.Models
{
    public class RabbitMqSettings
    {
        public string Host { get; set; } = string.Empty;
        public string VirtualHost { get; set; } = "/";
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
