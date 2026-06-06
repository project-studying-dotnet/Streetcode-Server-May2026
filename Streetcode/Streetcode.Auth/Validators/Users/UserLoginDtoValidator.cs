using FluentValidation;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Resources;

namespace Streetcode.Auth.Validators.Users
{
    public class UserLoginDtoValidator : AbstractValidator<UserLoginDto>
    {
        private const int MaxLoginLength = 20;
        private const int MaxPasswordLength = 20;
        public UserLoginDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage(ErrorMessages.LoginIsRequired)
                .MaximumLength(MaxLoginLength)
                .WithMessage(string.Format(ErrorMessages.LoginMustNotExceedCharacters, MaxLoginLength));

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(ErrorMessages.PasswordIsRequired)
                .MaximumLength(MaxPasswordLength)
                .WithMessage(string.Format(ErrorMessages.PasswordMustNotExceedCharacters, MaxPasswordLength));
        }
    }
}