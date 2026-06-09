using MailKit.Net.Smtp;
using MimeKit;
using Streetcode.EmailService.Interfaces;

namespace Streetcode.EmailService.Services;

public class SmtpClientWrapper : ISmtpClientWrapper
{
    private readonly SmtpClient _client = new();

    public bool IsConnected => _client.IsConnected;

    public Task ConnectAsync(string host, int port, bool useSsl) =>
        _client.ConnectAsync(host, port, useSsl);

    public void RemoveAuthenticationMechanism(string mechanism) =>
        _client.AuthenticationMechanisms.Remove(mechanism);

    public Task AuthenticateAsync(string userName, string password) =>
        _client.AuthenticateAsync(userName, password);

    public Task SendAsync(MimeMessage message) =>
        _client.SendAsync(message);

    public Task DisconnectAsync(bool quit) =>
        _client.DisconnectAsync(quit);

    public ValueTask DisposeAsync()
    {
        _client.Dispose();
        GC.SuppressFinalize(this);

        return ValueTask.CompletedTask;
    }
}