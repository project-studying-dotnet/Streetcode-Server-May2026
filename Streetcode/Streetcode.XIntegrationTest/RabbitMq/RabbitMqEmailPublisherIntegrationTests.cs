using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Streetcode.BLL.Contracts;
using Streetcode.BLL.Services.Email;
using Streetcode.BLL.Settings;
using Testcontainers.RabbitMq;
using Xunit;

namespace Streetcode.XIntegrationTest.RabbitMq;

public sealed class RabbitMqEmailPublisherIntegrationTests : IAsyncLifetime
{
    private const string RabbitMqUserName = "guest";
    private const string RabbitMqPassword = "guest";
    private const string QueueName = "email-queue";
    private const string ReceiverEmail = "receiver@test.com";
    private const string SenderEmail = "sender@test.com";
    private const string EmailSubject = "Integration test";
    private const string EmailContent = "Hello from RabbitMQ integration test";

    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder("rabbitmq:3.13-management")
        .WithUsername(RabbitMqUserName)
        .WithPassword(RabbitMqPassword)
        .Build();

    public Task InitializeAsync()
    {
        return _rabbitMqContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _rabbitMqContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task PublishAsync_Should_Publish_Email_Message_To_RabbitMq()
    {
        var settings = Options.Create(new RabbitMqSettings
        {
            HostName = _rabbitMqContainer.Hostname,
            Port = _rabbitMqContainer.GetMappedPublicPort(5672),
            UserName = RabbitMqUserName,
            Password = RabbitMqPassword,
            QueueName = QueueName,
        });

        var publisher = new RabbitMqEmailPublisher(settings);

        var message = new EmailMessageContract
        {
            To = new List<string> { ReceiverEmail },
            From = SenderEmail,
            Subject = EmailSubject,
            Content = EmailContent,
        };

        await publisher.PublishAsync(message);

        var receivedMessage = ReadMessageFromQueue(settings.Value);

        receivedMessage.Should().NotBeNull();
        receivedMessage!.From.Should().Be(message.From);
        receivedMessage.Subject.Should().Be(message.Subject);
        receivedMessage.Content.Should().Be(message.Content);
        receivedMessage.To.Should().ContainSingle(ReceiverEmail);
    }

    private static EmailMessageContract? ReadMessageFromQueue(RabbitMqSettings settings)
    {
        var factory = new ConnectionFactory
        {
            HostName = settings.HostName,
            Port = settings.Port,
            UserName = settings.UserName,
            Password = settings.Password,
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        var result = channel.BasicGet(settings.QueueName, autoAck: true);

        if (result is null)
        {
            return null;
        }

        var json = Encoding.UTF8.GetString(result.Body.ToArray());

        return JsonSerializer.Deserialize<EmailMessageContract>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
    }
}