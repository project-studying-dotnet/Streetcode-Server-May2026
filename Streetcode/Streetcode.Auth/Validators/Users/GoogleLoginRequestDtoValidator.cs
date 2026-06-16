using FluentValidation;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Resources;

namespace Streetcode.Auth.Validators.Users
{
    public class GoogleLoginRequestDtoValidator : AbstractValidator<GoogleLoginRequestDto>
    {
        public GoogleLoginRequestDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(ErrorMessages.EmailIsRequired)
                .EmailAddress().WithMessage(ErrorMessages.InvalidEmailFormat);

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(ErrorMessages.NameIsRequired)
                .MaximumLength(50).WithMessage(string.Format(ErrorMessages.NameMustNotExceedCharacters, 50));

            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage(ErrorMessages.NameIsRequired)
                .MaximumLength(50).WithMessage(string.Format(ErrorMessages.NameMustNotExceedCharacters, 50));
        }
    }
}
