using FluentValidation;
using Streetcode.BLL.MediatR.Payment;

namespace Streetcode.BLL.Validators.Payment
{
    /// <summary>
    /// Validator for CreateInvoiceCommand.
    /// </summary>
    public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateInvoiceCommandValidator"/> class.
        /// </summary>
        public CreateInvoiceCommandValidator()
        {
            RuleFor(x => x.Payment)
                .NotNull()
                .WithMessage("Payment is required")
                .SetValidator(new PaymentDtoValidator());
        }
    }
}
