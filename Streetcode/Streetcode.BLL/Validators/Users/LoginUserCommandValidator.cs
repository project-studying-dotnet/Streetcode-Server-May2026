using FluentValidation;
using Streetcode.BLL.MediatR.Users.Login;
using Streetcode.BLL.Validators.Users;

namespace Streetcode.BLL.Validators.Users;

public class LoginUserCommandValidator
    : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.loginRequest)
            .SetValidator(new UserLoginDtoValidator());
    }
}