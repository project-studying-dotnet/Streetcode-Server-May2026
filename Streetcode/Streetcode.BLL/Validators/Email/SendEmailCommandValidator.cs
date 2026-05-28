using FluentValidation;
using Streetcode.BLL.MediatR.Email;
using Streetcode.BLL.Resources;

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
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Email)
                .NotNull()
                .WithMessage(ErrorMessages.EmailPayloadIsRequired)
                .SetValidator(new EmailDtoValidator());
        }
    }
}
