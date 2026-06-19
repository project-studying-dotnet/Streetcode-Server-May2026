using FluentValidation;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Users
{
    public class ForgotPasswordDtoValidator : AbstractValidator<ForgotPasswordDto>
    {
        public ForgotPasswordDtoValidator()
        {
            RuleFor(x => x.Email)
               .NotEmpty()
               .WithMessage(ErrorMessages.EmailIsRequired)
               .EmailAddress()
               .WithMessage(ErrorMessages.InvalidEmailFormat);
        }
    }
}
