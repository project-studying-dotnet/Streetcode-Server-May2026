using FluentValidation;
using Streetcode.BLL.DTO.Email;

namespace Streetcode.BLL.Validators.Email
{
    /// <summary>
    /// Validator for EmailDTO.
    /// </summary>
    public class EmailDtoValidator : AbstractValidator<EmailDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailDtoValidator"/> class.
        /// </summary>
        public EmailDtoValidator()
        {
            RuleFor(x => x.From)
                .MaximumLength(80)
                .WithMessage("From must not exceed 80 characters");

            RuleFor(x => x.Content)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Content is required")
                .MinimumLength(1)
                .WithMessage("Content must contain at least 1 character")
                .MaximumLength(500)
                .WithMessage("Content must not exceed 500 characters");
        }
    }
}
