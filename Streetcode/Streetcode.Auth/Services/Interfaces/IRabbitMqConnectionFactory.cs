using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Streetcode.Common.Models;

namespace Streetcode.Auth.Services.Interfaces
{
    public interface IRabbitMqConnectionFactory
    {
        public IConnection CreateConnection();
    }
}
