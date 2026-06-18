using FluentValidation;
using Streetcode.Auth.MediatR.Users.Register;

namespace Streetcode.Auth.Validators.Users;

public class RegisterUserCommandValidator
    : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.registerRequest)
            .SetValidator(new UserRegisterDtoValidator());
    }
}