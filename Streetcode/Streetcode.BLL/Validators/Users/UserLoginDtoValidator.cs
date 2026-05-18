using FluentValidation;
using Streetcode.BLL.DTO.Users;

namespace Streetcode.BLL.Validators.Users
{
    /// <summary>
    /// Validator for UserLoginDTO.
    /// </summary>
    public class UserLoginDtoValidator : AbstractValidator<UserLoginDTO>
    {
        private const int LoginMaxLength = 20;
        private const int PasswordMaxLength = 20;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserLoginDtoValidator"/> class.
        /// </summary>
        public UserLoginDtoValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage("Login is required")
                .MaximumLength(LoginMaxLength)
                .WithMessage($"Login must not exceed {LoginMaxLength} characters");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required")
                .MaximumLength(PasswordMaxLength)
                .WithMessage($"Password must not exceed {PasswordMaxLength} characters");
        }
    }
}