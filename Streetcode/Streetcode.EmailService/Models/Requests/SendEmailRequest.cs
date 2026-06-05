using System.ComponentModel.DataAnnotations;

namespace Streetcode.EmailService.Models.Requests;

public class SendEmailRequest
{
    [Required]
    [MinLength(1)]
    public List<string> To { get; set; } = [];

    [Required]
    [EmailAddress]
    public string From { get; set; } = string.Empty;

    [Required]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;
}