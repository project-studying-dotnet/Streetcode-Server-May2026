using FluentValidation;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Users
{
    public class GoogleLoginRequestDtoValidator : AbstractValidator<GoogleLoginRequestDto>
    {
        public GoogleLoginRequestDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(ErrorMessages.EmailIsRequired)
                .EmailAddress().WithMessage(ErrorMessages.InvalidEmailFormat);

            RuleFor(x => x.Name).MustBeValidName();
            RuleFor(x => x.Surname).MustBeValidName();
        }
    }
}
