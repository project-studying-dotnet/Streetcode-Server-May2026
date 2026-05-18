using FluentValidation;
using Streetcode.BLL.DTO.Payment;

namespace Streetcode.BLL.Validators.Payment
{
    /// <summary>
    /// Validator for PaymentDTO.
    /// </summary>
    public class PaymentDtoValidator : AbstractValidator<PaymentDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentDtoValidator"/> class.
        /// </summary>
        public PaymentDtoValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0);

            RuleFor(x => x.RedirectUrl)
                .MaximumLength(500)
                .Must(uri => string.IsNullOrWhiteSpace(uri) || Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .WithMessage("RedirectUrl must be a valid absolute URL");
        }
    }
}