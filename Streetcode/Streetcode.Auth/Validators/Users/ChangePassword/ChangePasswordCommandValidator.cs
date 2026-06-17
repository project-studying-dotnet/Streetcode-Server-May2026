using FluentValidation;
using Streetcode.Auth.Models.MediatR.Users.ChangePassword;
using Streetcode.Auth.Resources;

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
                .NotEmpty().WithMessage(string.Format(ErrorMessages.FieldIsRequired, "New password"))
                .MinimumLength(MinPasswordLength)
                .WithMessage(string.Format(ErrorMessages.PasswordTooShort, MinPasswordLength))
                .NotEqual(x => x.ChangePasswordRequest.CurrentPassword)
                .WithMessage(ErrorMessages.PasswordCannotBeSameAsCurrent);

            RuleFor(x => x.ChangePasswordRequest.ConfirmNewPassword)
                .NotEmpty().WithMessage(string.Format(ErrorMessages.FieldIsRequired, "Confirmation password"))
                .Equal(x => x.ChangePasswordRequest.NewPassword)
                .WithMessage(ErrorMessages.PasswordsDoNotMatch);
        }
    }
}
