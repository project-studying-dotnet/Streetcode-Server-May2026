using Streetcode.EmailService.Interfaces;

namespace Streetcode.EmailService.Services;

public class SmtpClientFactory : ISmtpClientFactory
{
    public ISmtpClientWrapper CreateClient() => new SmtpClientWrapper();
}