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
                .RequiredWithMaxLength(MaxNameLength, ErrorMessages.NameIsRequired, ErrorMessages.NameMustNotExceedCharacters);

            RuleFor(x => x.Surname)
                .RequiredWithMaxLength(MaxNameLength, ErrorMessages.SurnameIsRequired, ErrorMessages.SurnameMustNotExceedCharacters);

            RuleFor(x => x.Email)
                .ValidEmail(MaxEmailLength, ErrorMessages.EmailIsRequired, ErrorMessages.InvalidEmailFormat, ErrorMessages.EmailMustNotExceedCharacters);

            RuleFor(x => x.Password)
                .ValidPassword(MinPasswordLength, MaxPasswordLength, ErrorMessages.PasswordIsRequired, ErrorMessages.PasswordMustBeAtLeastCharacters, ErrorMessages.PasswordMustNotExceedCharacters);

            RuleFor(x => x.PasswordConfirmation)
                .NotEmpty().WithMessage(ErrorMessages.PasswordConfirmationIsRequired)
                .Equal(x => x.Password).WithMessage(ErrorMessages.PasswordsDoNotMatch);
        }
    }
}