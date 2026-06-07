using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Streetcode.BLL.Contracts;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.BLL.Settings;
using Microsoft.Extensions.Options;

namespace Streetcode.BLL.Services.Email;

public class RabbitMqEmailPublisher : IEmailPublisher
{
    private readonly RabbitMqSettings _settings;
    private readonly IRabbitMqConnectionFactory _connectionFactory;

    public RabbitMqEmailPublisher(
        IOptions<RabbitMqSettings> settings,
        IRabbitMqConnectionFactory connectionFactory)
    {
        _settings = settings.Value;
        _connectionFactory = connectionFactory;
    }

    public Task PublishAsync(
        EmailMessageContract message,
        CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: _settings.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        channel.BasicPublish(
            exchange: string.Empty,
            routingKey: _settings.QueueName,
            basicProperties: null,
            body: body);

        return Task.CompletedTask;
    }
}