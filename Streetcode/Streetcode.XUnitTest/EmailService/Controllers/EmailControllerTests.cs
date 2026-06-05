using FluentAssertions;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Streetcode.EmailService.Controllers;
using Streetcode.EmailService.Models.Requests;
using Streetcode.XUnitTest.EmailService.Constants;

namespace Streetcode.XUnitTest.EmailService.Controllers;

public class EmailControllerTests
{
    private readonly Mock<IBackgroundJobClient> _backgroundJobClientMock = new();
    private readonly EmailController _controller;

    public EmailControllerTests()
    {
        _controller = new EmailController(_backgroundJobClientMock.Object);
    }

    [Fact]
    public void Send_ShouldReturnAccepted_WhenEmailIsQueued()
    {
        // Arrange
        var request = CreateRequest();

        _backgroundJobClientMock
            .Setup(client => client.Create(
                It.IsAny<Hangfire.Common.Job>(),
                It.IsAny<Hangfire.States.IState>()))
            .Returns("job-id");

        // Act
        var result = _controller.Send(request);

        // Assert
        result.Should().BeOfType<AcceptedResult>();

        _backgroundJobClientMock.Verify(
            client => client.Create(
                It.IsAny<Hangfire.Common.Job>(),
                It.IsAny<Hangfire.States.IState>()),
            Times.Once);
    }

    private static SendEmailRequest CreateRequest() => new()
    {
        To = [EmailTestConstants.TestEmail],
        From = EmailTestConstants.FromEmail,
        Subject = EmailTestConstants.Subject,
        Content = EmailTestConstants.Content,
    };
}