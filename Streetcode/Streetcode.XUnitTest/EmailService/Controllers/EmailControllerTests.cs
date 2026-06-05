using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Streetcode.EmailService.Controllers;
using Streetcode.EmailService.Interfaces;
using Streetcode.EmailService.Models;
using Streetcode.XUnitTest.EmailService.Constants;

namespace Streetcode.EmailService.XUnitTest.Controllers;

public class EmailControllerTests
{
    private readonly Mock<IEmailService> _emailServiceMock = new();

    [Fact]
    public async Task Send_ShouldReturnOk_WhenEmailSentSuccessfully()
    {
        var controller = new EmailController(_emailServiceMock.Object);
        var message = CreateMessage();

        _emailServiceMock
            .Setup(x => x.SendEmailAsync(message))
            .ReturnsAsync(true);

        var result = await controller.Send(message);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Send_ShouldReturnBadRequest_WhenEmailSendingFails()
    {
        var controller = new EmailController(_emailServiceMock.Object);
        var message = CreateMessage();

        _emailServiceMock
            .Setup(x => x.SendEmailAsync(message))
            .ReturnsAsync(false);

        var result = await controller.Send(message);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    private static Message CreateMessage() => new()
    {
        To = [EmailTestConstants.TestEmail],
        From = EmailTestConstants.FromEmail,
        Subject = EmailTestConstants.Subject,
        Content = EmailTestConstants.Content,
    };
}