using System.Text;
using System.Text.Json;
using FluentAssertions;
using Moq;
using RabbitMQ.Client;
using Streetcode.Auth.Services;
using Streetcode.Auth.Services.Interfaces;
using Xunit;

public class RabbitMqPublisherTests
{
    [Fact]
    public async Task PublishAsync_Should_Publish_Message_To_Queue()
    {
        // Arrange
        var queue = "test-queue";

        var message = new { Text = "Hello" };
        var json = JsonSerializer.Serialize(message);
        var expectedBody = Encoding.UTF8.GetBytes(json);

        var connectionMock = new Mock<IConnection>();
        var channelMock = new Mock<IModel>();
        var factoryMock = new Mock<IRabbitMqConnectionFactory>();

        ReadOnlyMemory<byte>? capturedBody = null;

        connectionMock
            .Setup(x => x.CreateModel())
            .Returns(channelMock.Object);

        factoryMock
            .Setup(x => x.CreateConnection())
            .Returns(connectionMock.Object);

        channelMock
            .Setup(x => x.BasicPublish(
                "",
                queue,
                false,
                null,
                It.IsAny<ReadOnlyMemory<byte>>()))
            .Callback<string, string, bool, IBasicProperties, ReadOnlyMemory<byte>>(
                (_, _, _, _, body) =>
                {
                    capturedBody = body;
                });

        var publisher = new RabbitMqPublisher(factoryMock.Object);

        // Act
        await publisher.PublishAsync(queue, message);

        // Assert
        channelMock.Verify(
            x => x.QueueDeclare(
                queue,
                true,
                false,
                false,
                null),
            Times.Once);

        capturedBody.Should().NotBeNull();

        Encoding.UTF8
            .GetString(capturedBody!.Value.Span)
            .Should()
            .Be(json);
    }
}