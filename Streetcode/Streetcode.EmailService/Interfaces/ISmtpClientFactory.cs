namespace Streetcode.EmailService.Interfaces;

public interface ISmtpClientFactory
{
    ISmtpClientWrapper CreateClient();
}