using FluentValidation;
using Streetcode.BLL.DTO.Users;

namespace Streetcode.BLL.Validators.Users
{
    /// <summary>
    /// Validator for UserDTO.
    /// </summary>
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        private const int NameMaxLength = 50;
        private const int LoginMaxLength = 20;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDtoValidator"/> class.
        /// </summary>
        public UserDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(NameMaxLength)
                .WithMessage($"Name must not exceed {NameMaxLength} characters");

            RuleFor(x => x.Surname)
                .NotEmpty()
                .WithMessage("Surname is required")
                .MaximumLength(NameMaxLength)
                .WithMessage($"Surname must not exceed {NameMaxLength} characters");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format");

            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("Login is required")
                .MaximumLength(LoginMaxLength)
                .WithMessage($"Login must not exceed {LoginMaxLength} characters");

            RuleFor(x => x.Role)
                .IsInEnum()
                .WithMessage("Invalid user role");
        }
    }
}
