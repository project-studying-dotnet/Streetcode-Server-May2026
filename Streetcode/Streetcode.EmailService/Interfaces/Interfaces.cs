using Streetcode.EmailService.Models;

namespace Streetcode.EmailService.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(Message message);
}