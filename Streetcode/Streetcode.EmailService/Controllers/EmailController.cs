using Microsoft.AspNetCore.Mvc;
using Streetcode.EmailService.Interfaces;
using Streetcode.EmailService.Models;

namespace Streetcode.EmailService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] Message message)
    {
        var result = await _emailService.SendEmailAsync(message);

        return result
        ? Ok("Email was sent successfully.")
        : BadRequest("Email was not sent. Check SMTP configuration.");
    }
}