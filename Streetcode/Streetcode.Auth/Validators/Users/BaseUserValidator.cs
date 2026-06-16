using FluentValidation;
using Streetcode.Auth.Resources;
using System.Linq.Expressions;

namespace Streetcode.Auth.Validators.Users
{
    public abstract class BaseUserValidator<T> : AbstractValidator<T>
    {
        protected void ApplyNameRules(Expression<Func<T, string>> expression, int maxLength)
        {
            RuleFor(expression).RequiredWithMaxLength(maxLength, ErrorMessages.NameIsRequired, ErrorMessages.NameMustNotExceedCharacters);
        }

        protected void ApplyLoginRules(Expression<Func<T, string>> expression, int maxLength)
        {
            RuleFor(expression).RequiredWithMaxLength(maxLength, ErrorMessages.LoginIsRequired, ErrorMessages.LoginMustNotExceedCharacters);
        }

        protected void ApplyPasswordRules(
            Expression<Func<T, string>> expression,
            int minLength,
            int maxLength,
            string requiredMsg,
            string minLengthMsg,
            string maxLengthMsg)
        {
            RuleFor(expression)
                .NotEmpty().WithMessage(requiredMsg)
                .MinimumLength(minLength).WithMessage(string.Format(minLengthMsg, minLength))
                .MaximumLength(maxLength).WithMessage(string.Format(maxLengthMsg, maxLength));
        }

        protected void ApplyStringRules(
            Expression<Func<T, string>> expression,
            int maxLength,
            string requiredMsg,
            string lengthMsg)
        {
            RuleFor(expression)
                .NotEmpty().WithMessage(requiredMsg)
                .MaximumLength(maxLength).WithMessage(string.Format(lengthMsg, maxLength));
        }
    }
}