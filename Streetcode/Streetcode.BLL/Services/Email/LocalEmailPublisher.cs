using System.Net;
using System.Net.Mail;
using Streetcode.BLL.Contracts;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.DAL.Entities.AdditionalContent.Email;

namespace Streetcode.BLL.Services.Email
{
    public class LocalEmailPublisher : ILocalEmailPublisher
    {
        private readonly EmailConfiguration _config;

        public LocalEmailPublisher(EmailConfiguration config)
        {
            _config = config;
        }

        public async Task PublishAsync(EmailMessageContract message, CancellationToken cancellationToken = default)
        {
            using var client = new SmtpClient();

            client.Host = _config.SmtpServer;
            client.Port = _config.Port;
            client.Credentials = new NetworkCredential(_config.UserName, _config.Password);
            client.EnableSsl = true;

            try
            {
                using var mailMessage = new MailMessage(_config.From, message.To[0])
                {
                    Subject = message.Subject,
                    Body = message.Content,
                    IsBodyHtml = true
                };

                await client.SendMailAsync(mailMessage, cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SMTP Error: {ex.Message}");
                throw;
            }
        }
    }
}