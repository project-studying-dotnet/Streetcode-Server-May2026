using FluentValidation;

namespace Streetcode.BLL.Validators
{
    public static class ValidationExtensions
    {
        // For Name and Surname
        public static IRuleBuilderOptions<T, string> RequiredWithMaxLength<T>(
            this IRuleBuilder<T, string> ruleBuilder,
            int maxLength,
            string requiredMessage,
            string lengthMessage)
        {
            return ruleBuilder
                .NotEmpty().WithMessage(requiredMessage)
                .MaximumLength(maxLength).WithMessage(string.Format(lengthMessage, maxLength));
        }

        // For Email
        public static IRuleBuilderOptions<T, string> ValidEmail<T>(
            this IRuleBuilder<T, string> ruleBuilder,
            int maxLength,
            string requiredMessage,
            string formatMessage,
            string lengthMessage)
        {
            return ruleBuilder
                .NotEmpty().WithMessage(requiredMessage)
                .EmailAddress().WithMessage(formatMessage)
                .MaximumLength(maxLength).WithMessage(string.Format(lengthMessage, maxLength));
        }

        // For Password
        public static IRuleBuilderOptions<T, string> ValidPassword<T>(
            this IRuleBuilder<T, string> ruleBuilder,
            int minLength,
            int maxLength,
            string requiredMessage,
            string minLengthMessage,
            string maxLengthMessage)
        {
            return ruleBuilder
                .NotEmpty().WithMessage(requiredMessage)
                .MinimumLength(minLength).WithMessage(string.Format(minLengthMessage, minLength))
                .MaximumLength(maxLength).WithMessage(string.Format(maxLengthMessage, maxLength));
        }
    }
}