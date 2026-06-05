using Microsoft.AspNetCore.Mvc;
using Streetcode.EmailService.Interfaces;
using Streetcode.EmailService.Models;
using Streetcode.EmailService.Models.Requests;

namespace Streetcode.EmailService.Controllers;

[ApiController]
[Route("api/email")]
public class EmailController : ControllerBase
{
    private const string EmailSentSuccessfully = "Email was sent successfully.";
    private const string EmailNotSent = "Email was not sent. Check SMTP configuration.";

    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] SendEmailRequest request)
    {
        var message = new Message
        {
            To = request.To,
            From = request.From,
            Subject = request.Subject,
            Content = request.Content,
        };

        var result = await _emailService.SendEmailAsync(message);

        return result
            ? Ok(EmailSentSuccessfully)
            : BadRequest(EmailNotSent);
    }
}