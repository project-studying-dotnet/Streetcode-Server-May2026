using FluentValidation;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Resources;

namespace Streetcode.Auth.Validators.Users
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        private const int MaxNameLength = 50;
        private const int MaxLoginLength = 20;

        public UserDtoValidator()
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
                .WithMessage(ErrorMessages.InvalidEmailFormat);

            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage(ErrorMessages.LoginIsRequired)
                .MaximumLength(MaxLoginLength)
                .WithMessage(string.Format(ErrorMessages.LoginMustNotExceedCharacters, MaxLoginLength));

            RuleFor(x => x.Role)
                .IsInEnum()
                .WithMessage(ErrorMessages.InvalidUserRole);
        }
    }
}
