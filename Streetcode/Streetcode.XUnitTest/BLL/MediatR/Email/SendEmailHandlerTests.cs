using System.Threading.Tasks;
using Moq;
using Streetcode.BLL.DTO.Email;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Email;
using Streetcode.DAL.Entities.AdditionalContent.Email;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Email;

public class SendEmailHandlerTests
{
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly SendEmailHandler _handler;

    public SendEmailHandlerTests()
    {
        _emailServiceMock = new Mock<IEmailService>();
        _loggerMock = new Mock<ILoggerService>();

        _handler = new SendEmailHandler(
            _emailServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Ok_When_Email_Is_Sent()
    {
        var command = CreateCommand();

        _emailServiceMock
            .Setup(s => s.SendEmailAsync(It.IsAny<Message>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);

        _emailServiceMock.Verify(
            x => x.SendEmailAsync(It.IsAny<Message>()),
            Times.Once);

        _loggerMock.Verify(
            x => x.LogError(It.IsAny<object>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Return_Fail_When_Email_Sending_Fails()
    {
        var command = CreateCommand();

        _emailServiceMock
            .Setup(s => s.SendEmailAsync(It.IsAny<Message>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailed);

        _emailServiceMock.Verify(
            x => x.SendEmailAsync(It.IsAny<Message>()),
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