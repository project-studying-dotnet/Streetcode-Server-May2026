using FluentValidation;

namespace Streetcode.Auth.Validators
{
    public static class ValidationExtensions
    {
        private static IRuleBuilder<T, string> ApplyBaseRules<T>(
            this IRuleBuilder<T, string> ruleBuilder,
            string requiredMessage)
        {
            return ruleBuilder.NotEmpty().WithMessage(requiredMessage);
        }

        public static IRuleBuilderOptions<T, string> RequiredWithMaxLength<T>(
            this IRuleBuilder<T, string> ruleBuilder,
            int maxLength,
            string requiredMessage,
            string lengthMessage)
        {
            return ruleBuilder.ApplyBaseRules(requiredMessage)
                .MaximumLength(maxLength).WithMessage(string.Format(lengthMessage, maxLength));
        }

        public static IRuleBuilderOptions<T, string> ValidEmail<T>(
            this IRuleBuilder<T, string> ruleBuilder,
            int maxLength,
            string requiredMessage,
            string formatMessage,
            string lengthMessage)
        {
            return ruleBuilder.ApplyBaseRules(requiredMessage)
                .EmailAddress().WithMessage(formatMessage)
                .MaximumLength(maxLength).WithMessage(string.Format(lengthMessage, maxLength));
        }

        public static IRuleBuilderOptions<T, string> Required<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        string fieldName,
        string messageTemplate)
        {
            return ruleBuilder.NotEmpty().WithMessage(string.Format(messageTemplate, fieldName));
        }

        public static IRuleBuilderOptions<T, string> ValidPassword<T>(
            this IRuleBuilder<T, string> ruleBuilder,
            int minLength,
            int maxLength,
            string requiredMessage,
            string minLengthMessage,
            string maxLengthMessage)
        {
            return ruleBuilder.ApplyBaseRules(requiredMessage)
                .MinimumLength(minLength).WithMessage(string.Format(minLengthMessage, minLength))
                .MaximumLength(maxLength).WithMessage(string.Format(maxLengthMessage, maxLength));
        }
    }
}