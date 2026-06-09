using MimeKit;

namespace Streetcode.EmailService.Interfaces;

public interface ISmtpClientWrapper : IAsyncDisposable
{
    bool IsConnected { get; }

    Task ConnectAsync(string host, int port, bool useSsl);

    void RemoveAuthenticationMechanism(string mechanism);

    Task AuthenticateAsync(string userName, string password);

    Task SendAsync(MimeMessage message);

    Task DisconnectAsync(bool quit);
}