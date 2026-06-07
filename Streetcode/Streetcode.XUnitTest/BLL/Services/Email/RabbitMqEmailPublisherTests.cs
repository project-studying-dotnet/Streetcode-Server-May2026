using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMQ.Client;
using Streetcode.BLL.Contracts;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.BLL.Services.Email;
using Streetcode.BLL.Settings;
using Streetcode.XUnitTest.EmailService.Constants;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Services.Email;

public class RabbitMqEmailPublisherTests
{
    private readonly Mock<IRabbitMqConnectionFactory> _connectionFactoryMock;
    private readonly Mock<IConnection> _connectionMock;
    private readonly Mock<IModel> _channelMock;
    private readonly RabbitMqEmailPublisher _publisher;

    public RabbitMqEmailPublisherTests()
    {
        _connectionFactoryMock = new Mock<IRabbitMqConnectionFactory>();
        _connectionMock = new Mock<IConnection>();
        _channelMock = new Mock<IModel>();

        var settings = Options.Create(new RabbitMqSettings
        {
            HostName = EmailTestConstants.RabbitMqHostName,
            Port = EmailTestConstants.RabbitMqPort,
            UserName = EmailTestConstants.RabbitMqUserName,
            Password = EmailTestConstants.RabbitMqPassword,
            QueueName = EmailTestConstants.RabbitMqQueueName,
        });

        _connectionFactoryMock
            .Setup(factory => factory.CreateConnection())
            .Returns(_connectionMock.Object);

        _connectionMock
            .Setup(connection => connection.CreateModel())
            .Returns(_channelMock.Object);

        _publisher = new RabbitMqEmailPublisher(
            settings,
            _connectionFactoryMock.Object);
    }

    [Fact]
    public async Task PublishAsync_Should_Declare_Queue_And_Publish_Message()
    {
        var message = new EmailMessageContract
        {
            To = new List<string> { EmailTestConstants.TestEmail },
            From = EmailTestConstants.FromEmail,
            Subject = EmailTestConstants.Subject,
            Content = EmailTestConstants.Content,
        };

        await _publisher.PublishAsync(message, CancellationToken.None);

        _channelMock.Verify(
            channel => channel.QueueDeclare(
                EmailTestConstants.RabbitMqQueueName,
                true,
                false,
                false,
                null),
            Times.Once);

        _channelMock.Verify(
            channel => channel.BasicPublish(
                string.Empty,
                EmailTestConstants.RabbitMqQueueName,
                false,
                null,
                It.Is<ReadOnlyMemory<byte>>(body =>
                    Encoding.UTF8.GetString(body.ToArray()).Contains(EmailTestConstants.Content))),
            Times.Once);
    }
}