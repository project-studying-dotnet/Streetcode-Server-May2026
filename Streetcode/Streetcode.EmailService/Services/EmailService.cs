using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Streetcode.EmailService.Interfaces;
using Streetcode.EmailService.Models;

namespace Streetcode.EmailService.Services;

public class EmailService : IEmailService
{
    private readonly EmailConfiguration _emailConfig;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IOptions<EmailConfiguration> emailConfig,
        ILogger<EmailService> logger)
    {
        _emailConfig = emailConfig.Value;
        _logger = logger;
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
        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);

            client.AuthenticationMechanisms.Remove("XOAUTH2");

            await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);
            await client.SendAsync(mailMessage);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email.");
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