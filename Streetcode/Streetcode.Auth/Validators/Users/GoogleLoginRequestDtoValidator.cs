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
                .ValidEmail(
                    maxLength: 255,
                    requiredMessage: ErrorMessages.EmailIsRequired,
                    formatMessage: ErrorMessages.InvalidEmailFormat,
                    lengthMessage: ErrorMessages.EmailMustNotExceedCharacters);

            RuleFor(x => x.Name)
                .RequiredWithMaxLength(
                    maxLength: 50,
                    requiredMessage: ErrorMessages.NameIsRequired,
                    lengthMessage: ErrorMessages.NameMustNotExceedCharacters);

            RuleFor(x => x.Surname)
                .RequiredWithMaxLength(
                    maxLength: 50,
                    requiredMessage: ErrorMessages.NameIsRequired,
                    lengthMessage: ErrorMessages.NameMustNotExceedCharacters);
        }
    }
}
