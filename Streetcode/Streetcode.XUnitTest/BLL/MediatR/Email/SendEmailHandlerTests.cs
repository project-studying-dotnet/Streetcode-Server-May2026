using Moq;
using Streetcode.BLL.Contracts;
using Streetcode.BLL.DTO.Email;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Email;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Email;

public class SendEmailHandlerTests
{
    private readonly Mock<IEmailPublisher> _emailPublisherMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly SendEmailHandler _handler;

    public SendEmailHandlerTests()
    {
        _emailPublisherMock = new Mock<IEmailPublisher>();
        _loggerMock = new Mock<ILoggerService>();

        _handler = new SendEmailHandler(
            _emailPublisherMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Ok_When_Email_Is_Published()
    {
        var command = CreateCommand();

        _emailPublisherMock
            .Setup(s => s.PublishAsync(
                It.IsAny<EmailMessageContract>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);

        _emailPublisherMock.Verify(
            x => x.PublishAsync(
                It.Is<EmailMessageContract>(message =>
                    message.To.Contains("streetcodeua@gmail.com") &&
                    message.From == command.Email.From &&
                    message.Subject == "FeedBack" &&
                    message.Content == command.Email.Content),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _loggerMock.Verify(
            x => x.LogError(It.IsAny<object>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_Fail_When_Email_Publishing_Fails()
    {
        var command = CreateCommand();

        _emailPublisherMock
            .Setup(s => s.PublishAsync(
                It.IsAny<EmailMessageContract>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("RabbitMQ error"));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);

        _emailPublisherMock.Verify(
            x => x.PublishAsync(
                It.IsAny<EmailMessageContract>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _loggerMock.Verify(
            x => x.LogError(It.IsAny<object>(), It.IsAny<string>()),
            Times.Once);
    }

    private static SendEmailCommand CreateCommand() =>
        new SendEmailCommand(new EmailDTO
        {
            From = "test@test.com",
            Content = "Hello",
        });
}