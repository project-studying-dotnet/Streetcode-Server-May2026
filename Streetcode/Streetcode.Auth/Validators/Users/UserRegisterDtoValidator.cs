using FluentValidation;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Resources;

namespace Streetcode.Auth.Validators.Users
{
    public class UserRegisterDtoValidator : BaseUserValidator<UserRegisterDto>
    {
        private const int MaxNameLength = 50;
        private const int MaxEmailLength = 256;
        private const int MinPasswordLength = 8;
        private const int MaxPasswordLength = 20;

        public UserRegisterDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            ApplyStringRules(x => x.Name, MaxNameLength, ErrorMessages.NameIsRequired, ErrorMessages.NameMustNotExceedCharacters);
            ApplyStringRules(x => x.Surname, MaxNameLength, ErrorMessages.SurnameIsRequired, ErrorMessages.SurnameMustNotExceedCharacters);

            RuleFor(x => x.Email)
                .ValidEmail(MaxEmailLength, ErrorMessages.EmailIsRequired, ErrorMessages.InvalidEmailFormat, ErrorMessages.EmailMustNotExceedCharacters);

            ApplyPasswordRules(x => x.Password, MinPasswordLength, MaxPasswordLength,
                ErrorMessages.PasswordIsRequired,
                ErrorMessages.PasswordMustBeAtLeastCharacters,
                ErrorMessages.PasswordMustNotExceedCharacters);

            RuleFor(x => x.PasswordConfirmation)
                .NotEmpty().WithMessage(ErrorMessages.PasswordConfirmationIsRequired)
                .Equal(x => x.Password).WithMessage(ErrorMessages.PasswordsDoNotMatch);
        }
    }
}