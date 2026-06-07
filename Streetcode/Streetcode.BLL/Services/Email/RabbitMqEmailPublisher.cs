using System.Text;
using System.Text.Json;
using System.Diagnostics.CodeAnalysis;
using RabbitMQ.Client;
using Streetcode.BLL.Contracts;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.BLL.Settings;
using Microsoft.Extensions.Options;
namespace Streetcode.BLL.Services.Email;

// RabbitMqEmailPublisher depends on a real RabbitMQ 
// broker and is intended to be covered by integration tests in task #199.
[ExcludeFromCodeCoverage]
public class RabbitMqEmailPublisher : IEmailPublisher
{
    private readonly RabbitMqSettings _settings;

    public RabbitMqEmailPublisher(IOptions<RabbitMqSettings> settings)
    {
        _settings = settings.Value;
    }

    public Task PublishAsync(
        EmailMessageContract message,
        CancellationToken cancellationToken = default)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password,
        };

        using var connection = factory.CreateConnection();
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