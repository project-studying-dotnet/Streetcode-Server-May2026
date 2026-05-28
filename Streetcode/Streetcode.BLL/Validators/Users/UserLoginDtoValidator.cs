using FluentValidation;
using Streetcode.BLL.DTO.Users;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Users
{
    /// <summary>
    /// Validator for UserLoginDTO.
    /// </summary>
    public class UserLoginDtoValidator : AbstractValidator<UserLoginDto>
    {
        private const int MaxLoginLength = 20;
        private const int MaxPasswordLength = 20;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserLoginDtoValidator"/> class.
        /// </summary>
        public UserLoginDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Login)
                .NotEmpty()
                .WithMessage(ErrorMessages.LoginIsRequired)
                .MaximumLength(MaxLoginLength)
                .WithMessage(string.Format(ErrorMessages.LoginMustNotExceedCharacters, MaxLoginLength));

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(ErrorMessages.PasswordIsRequired)
                .MaximumLength(MaxPasswordLength)
                .WithMessage(string.Format(ErrorMessages.PasswordMustNotExceedCharacters, MaxPasswordLength));
        }
    }
}