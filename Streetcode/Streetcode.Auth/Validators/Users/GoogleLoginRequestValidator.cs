using FluentValidation;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Resources;

namespace Streetcode.Auth.Validators.Users
{
    public class GoogleLoginRequestValidator : AbstractValidator<GoogleLoginRequest>
    {
        public GoogleLoginRequestValidator()
        {
            RuleFor(x => x.IdToken)
                .NotEmpty().WithMessage(ErrorMessages.GoogleIDTokenIsRequired)
                .MinimumLength(100).WithMessage(ErrorMessages.InvalidTokenFormat);
        }
    }
}
