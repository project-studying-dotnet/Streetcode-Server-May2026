using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MimeKit;
using Xunit;
using Streetcode.EmailService.Interfaces;
using Streetcode.EmailService.Models;
using Streetcode.XUnitTest.EmailService.Constants;
using EmailServiceImplementation = Streetcode.EmailService.Services.EmailService;

namespace Streetcode.XUnitTest.EmailService.Services;

public class EmailServiceTests
{
    private readonly Mock<ISmtpClientFactory> _smtpClientFactoryMock = new();
    private readonly Mock<ISmtpClientWrapper> _smtpClientMock = new();
    private readonly Mock<ILogger<EmailServiceImplementation>> _loggerMock = new();

    private readonly EmailServiceImplementation _emailService;

    public EmailServiceTests()
    {
        var options = Options.Create(new EmailConfiguration
        {
            From = EmailTestConstants.FromEmail,
            SmtpServer = EmailTestConstants.SmtpServer,
            Port = EmailTestConstants.Port,
            UserName = EmailTestConstants.UserName,
            Password = EmailTestConstants.Password,
        });

        _smtpClientFactoryMock
            .Setup(factory => factory.CreateClient())
            .Returns(_smtpClientMock.Object);

        _emailService = new EmailServiceImplementation(
            options,
            _loggerMock.Object,
            _smtpClientFactoryMock.Object);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldReturnTrue_WhenEmailWasSentSuccessfully()
    {
        // Arrange
        var message = CreateMessage();

        // Act
        var result = await _emailService.SendEmailAsync(message);

        // Assert
        result.Should().BeTrue();

        _smtpClientMock.Verify(
            client => client.ConnectAsync(EmailTestConstants.SmtpServer, EmailTestConstants.Port, true),
            Times.Once);

        _smtpClientMock.Verify(
            client => client.RemoveAuthenticationMechanism("XOAUTH2"),
            Times.Once);

        _smtpClientMock.Verify(
            client => client.AuthenticateAsync(EmailTestConstants.UserName, EmailTestConstants.Password),
            Times.Once);

        _smtpClientMock.Verify(
            client => client.SendAsync(It.IsAny<MimeMessage>()),
            Times.Once);
    }

    [Fact]
    public async Task SendEmailAsync_ShouldReturnFalse_WhenSmtpClientThrowsException()
    {
        // Arrange
        var message = CreateMessage();

        _smtpClientMock
            .Setup(client => client.SendAsync(It.IsAny<MimeMessage>()))
            .ThrowsAsync(new InvalidOperationException());

        // Act
        var result = await _emailService.SendEmailAsync(message);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task SendEmailAsync_ShouldDisconnect_WhenClientIsConnected()
    {
        // Arrange
        var message = CreateMessage();

        _smtpClientMock
            .Setup(client => client.IsConnected)
            .Returns(true);

        // Act
        await _emailService.SendEmailAsync(message);

        // Assert
        _smtpClientMock.Verify(
            client => client.DisconnectAsync(true),
            Times.Once);
    }

    private static Message CreateMessage() => new()
    {
        To = [EmailTestConstants.TestEmail],
        From = EmailTestConstants.FromEmail,
        Subject = EmailTestConstants.Subject,
        Content = EmailTestConstants.Content,
    };
}