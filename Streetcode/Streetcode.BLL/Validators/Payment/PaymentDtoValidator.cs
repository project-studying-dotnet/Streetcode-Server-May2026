using FluentValidation;
using Streetcode.BLL.DTO.Payment;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Payment
{
    /// <summary>
    /// Validator for PaymentDTO.
    /// </summary>
    public class PaymentDtoValidator : AbstractValidator<PaymentDTO>
    {
        private const int MaxUrlLength = 500;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentDtoValidator"/> class.
        /// </summary>
        public PaymentDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.AmountMustBeGreaterThanZero);

            RuleFor(x => x.RedirectUrl)
                .MaximumLength(MaxUrlLength)
                .WithMessage(string.Format(ErrorMessages.UrlMustNotExceedCharacters, MaxUrlLength))
                .Must(uri => string.IsNullOrWhiteSpace(uri) || Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .WithMessage(ErrorMessages.UrlMustBeValidAbsoluteUrl);
        }
    }
}