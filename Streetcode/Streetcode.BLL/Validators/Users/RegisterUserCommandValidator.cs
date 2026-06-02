using FluentValidation;
using Streetcode.BLL.MediatR.Users.Register;
using Streetcode.BLL.Validators.Users;

public class RegisterUserCommandValidator
    : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.registerRequest)
            .SetValidator(new UserRegisterDtoValidator());
    }
}