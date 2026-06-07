using RabbitMQ.Client;

namespace Streetcode.BLL.Interfaces.Email;

public interface IRabbitMqConnectionFactory
{
    IConnection CreateConnection();
}