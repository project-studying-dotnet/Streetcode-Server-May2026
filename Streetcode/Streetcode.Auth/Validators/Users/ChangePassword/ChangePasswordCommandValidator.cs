using FluentValidation;
using Streetcode.Auth.Models.MediatR.Users.ChangePassword;
using Streetcode.Auth.Resources;
using Streetcode.Auth.Validators;

namespace Streetcode.Auth.Models.Validators.Users.ChangePassword
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public const int MinPasswordLength = 8;
        public const int MaxPasswordLength = 64;
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.ChangePasswordRequest.CurrentPassword)
                  .NotEmpty().WithMessage(string.Format(ErrorMessages.FieldIsRequired, "Current password"));

            RuleFor(x => x.ChangePasswordRequest.NewPassword)
                .ValidPassword(
                    MinPasswordLength,
                    MaxPasswordLength,
                    string.Format(ErrorMessages.FieldIsRequired, "New password"),
                    ErrorMessages.PasswordTooShort,
                    "Password must not exceed {0} characters")
                .NotEqual(x => x.ChangePasswordRequest.CurrentPassword)
                .WithMessage(ErrorMessages.PasswordCannotBeSameAsCurrent);

            RuleFor(x => x.ChangePasswordRequest.ConfirmNewPassword)
                .NotEmpty().WithMessage(string.Format(ErrorMessages.FieldIsRequired, "Confirmation password"))
                .Equal(x => x.ChangePasswordRequest.NewPassword)
                .WithMessage(ErrorMessages.PasswordsDoNotMatch);
        }
    }
}
