using FluentValidation;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Users
{
    public class UserRegisterDtoValidator : AbstractValidator<UserRegisterDto>
    {
        private const int MaxNameLength = 50;
        private const int MaxEmailLength = 256;
        private const int MinPasswordLength = 8;
        private const int MaxPasswordLength = 20;

        public UserRegisterDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(ErrorMessages.NameIsRequired)
                .MaximumLength(MaxNameLength)
                .WithMessage(string.Format(ErrorMessages.NameMustNotExceedCharacters, MaxNameLength));

            RuleFor(x => x.Surname)
                .NotEmpty()
                .WithMessage(ErrorMessages.SurnameIsRequired)
                .MaximumLength(MaxNameLength)
                .WithMessage(string.Format(ErrorMessages.SurnameMustNotExceedCharacters, MaxNameLength));

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(ErrorMessages.EmailIsRequired)
                .EmailAddress()
                .WithMessage(ErrorMessages.InvalidEmailFormat)
                .MaximumLength(MaxEmailLength)
                .WithMessage(string.Format(ErrorMessages.EmailMustNotExceedCharacters, MaxEmailLength));

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(ErrorMessages.PasswordIsRequired)
                .MinimumLength(MinPasswordLength)
                .WithMessage(string.Format(ErrorMessages.PasswordMustBeAtLeastCharacters, MinPasswordLength))
                .MaximumLength(MaxPasswordLength)
                .WithMessage(string.Format(ErrorMessages.PasswordMustNotExceedCharacters, MaxPasswordLength));

            RuleFor(x => x.PasswordConfirmation)
                .NotEmpty()
                .WithMessage(ErrorMessages.PasswordConfirmationIsRequired)
                .Equal(x => x.Password)
                .WithMessage(ErrorMessages.PasswordsDoNotMatch);
        }
    }
}
