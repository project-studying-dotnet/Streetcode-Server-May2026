using FluentValidation;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Resources;

namespace Streetcode.Auth.Validators.Users
{
    public class UserLoginDtoValidator : BaseUserValidator<UserLoginDto>
    {
        private const int MaxLoginLength = 20;
        private const int MaxPasswordLength = 20;
        private const int MinPasswordLength = 8;

        public UserLoginDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            ApplyLoginRules(x => x.Login, MaxLoginLength);

            ApplyPasswordRules(x => x.Password, MinPasswordLength, MaxPasswordLength,
                 ErrorMessages.PasswordIsRequired,
                 ErrorMessages.PasswordMustBeAtLeastCharacters,
                 ErrorMessages.PasswordMustNotExceedCharacters);
        }
    }
}