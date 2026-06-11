using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Streetcode.Auth.Services.Interfaces;

namespace Streetcode.Auth.Services
{
    public class RabbitMqPublisher : IRabbitMqPublisher
    {
        private readonly IRabbitMqConnectionFactory _factory;

        public RabbitMqPublisher(IRabbitMqConnectionFactory factory)
        {
            _factory = factory;
        }

        public Task PublishAsync<T>(string queue, T message)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublish(
                exchange: "",
                routingKey: queue,
                basicProperties: null,
                body: body);

            return Task.CompletedTask;
        }
    }
}