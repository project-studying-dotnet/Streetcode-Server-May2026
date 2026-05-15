using FluentValidation;
using Streetcode.BLL.MediatR.Email;

namespace Streetcode.BLL.Validators.Email
{
    /// <summary>
    /// Validator for SendEmailCommand.
    /// </summary>
    public class SendEmailCommandValidator : AbstractValidator<SendEmailCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendEmailCommandValidator"/> class.
        /// </summary>
        public SendEmailCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotNull()
                .WithMessage("Email payload is required")
                .SetValidator(new EmailDtoValidator());
        }
    }
}
