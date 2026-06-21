using FluentValidation;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Users
{
    public static class ValidationExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBeValidName<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage(ErrorMessages.NameIsRequired)
                .MaximumLength(50).WithMessage(string.Format(ErrorMessages.NameMustNotExceedCharacters, 50));
        }
    }
}
