using FluentValidation;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Users
{
    public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(ErrorMessages.EmailIsRequired)
                .EmailAddress().WithMessage(ErrorMessages.InvalidEmailFormat);

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage(ErrorMessages.PasswordIsRequired)
                .MinimumLength(8).WithMessage(string.Format(ErrorMessages.PasswordMinimumLength, 8))
                .Matches(@"[A-Z]").WithMessage(ErrorMessages.PasswordUppercaseRequired)
                .Matches(@"[a-z]").WithMessage(ErrorMessages.PasswordLowercaseRequired)
                .Matches(@"[0-9]").WithMessage(ErrorMessages.PasswordNumberRequired);
        }
    }
}
