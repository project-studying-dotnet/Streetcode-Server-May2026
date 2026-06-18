using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Streetcode.Auth.Services.Interfaces;
using Streetcode.Common.Models;

namespace Streetcode.Auth.Services
{
    public class RabbitMqConnectionFactory : IRabbitMqConnectionFactory
    {
        private readonly RabbitMqSettings _settings;

        public RabbitMqConnectionFactory(IOptions<RabbitMqSettings> options)
        {
            _settings = options.Value;
        }

        public IConnection CreateConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password,
                VirtualHost = _settings.VirtualHost
            };

            return factory.CreateConnection();
        }
    }
}