using FluentValidation;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Users
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
