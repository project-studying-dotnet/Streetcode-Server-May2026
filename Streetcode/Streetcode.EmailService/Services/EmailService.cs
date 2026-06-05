using Microsoft.Extensions.Options;
using MimeKit;
using Streetcode.EmailService.Interfaces;
using Streetcode.EmailService.Models;

namespace Streetcode.EmailService.Services;

public class EmailService : IEmailService
{
    private const string XOAuth2 = "XOAUTH2";
    private const string FailedToSendEmailMessage = "Failed to send email.";

    private readonly EmailConfiguration _emailConfig;
    private readonly ILogger<EmailService> _logger;
    private readonly ISmtpClientFactory _smtpClientFactory;

    public EmailService(
        IOptions<EmailConfiguration> emailConfig,
        ILogger<EmailService> logger,
        ISmtpClientFactory smtpClientFactory)
    {
        _emailConfig = emailConfig.Value;
        _logger = logger;
        _smtpClientFactory = smtpClientFactory;
    }

    public async Task<bool> SendEmailAsync(Message message)
    {
        var mailMessage = CreateEmailMessage(message);

        return await SendAsync(mailMessage);
    }

    private MimeMessage CreateEmailMessage(Message message)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress(string.Empty, _emailConfig.From));

        emailMessage.To.AddRange(
            message.To.Select(email => new MailboxAddress(string.Empty, email)));

        emailMessage.Subject = message.Subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody =
                "<h2 style='color:black;'>" +
                $"Від: {message.From} <br>" +
                $"Текст: {message.Content}" +
                "</h2>",
        };

        emailMessage.Body = bodyBuilder.ToMessageBody();

        return emailMessage;
    }

    private async Task<bool> SendAsync(MimeMessage mailMessage)
    {
        await using var client = _smtpClientFactory.CreateClient();

        try
        {
            await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);

            client.RemoveAuthenticationMechanism(XOAuth2);

            await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);
            await client.SendAsync(mailMessage);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, FailedToSendEmailMessage);
            return false;
        }
        finally
        {
            if (client.IsConnected)
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}