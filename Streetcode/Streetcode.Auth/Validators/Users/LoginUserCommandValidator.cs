using FluentValidation;
using Streetcode.Auth.MediatR.Users.Login;

namespace Streetcode.Auth.Validators.Users;

public class LoginUserCommandValidator
    : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.loginRequest)
            .SetValidator(new UserLoginDtoValidator());
    }
}