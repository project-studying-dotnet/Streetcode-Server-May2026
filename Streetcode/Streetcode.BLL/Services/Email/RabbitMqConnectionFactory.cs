using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.BLL.Settings;

namespace Streetcode.BLL.Services.Email;

public class RabbitMqConnectionFactory : IRabbitMqConnectionFactory
{
    private readonly RabbitMqSettings _settings;

    public RabbitMqConnectionFactory(
        IOptions<RabbitMqSettings> settings)
    {
        _settings = settings.Value;
    }

    public IConnection CreateConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password,
        };

        return factory.CreateConnection();
    }
}