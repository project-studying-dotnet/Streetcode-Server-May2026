using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Streetcode.EmailService.Interfaces;
using Streetcode.EmailService.Models;
using Streetcode.EmailService.Models.Requests;

namespace Streetcode.EmailService.Controllers;

[ApiController]
[Route("api/email")]
public class EmailController : ControllerBase
{
    private const string EmailQueuedSuccessfully = "Email queued successfully.";

    private readonly IBackgroundJobClient _backgroundJobClient;

    public EmailController(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    [HttpPost("send")]
    public IActionResult Send([FromBody] SendEmailRequest request)
    {
        var message = new Message
        {
            To = request.To,
            From = request.From,
            Subject = request.Subject,
            Content = request.Content,
        };

        _backgroundJobClient.Enqueue<IEmailService>(
            emailService => emailService.SendEmailAsync(message));

        return Accepted(EmailQueuedSuccessfully);
    }
}