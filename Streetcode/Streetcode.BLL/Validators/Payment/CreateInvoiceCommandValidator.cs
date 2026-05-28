using FluentValidation;
using Streetcode.BLL.MediatR.Payment;
using Streetcode.BLL.Resources;

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
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Payment)
                .NotNull()
                .WithMessage(ErrorMessages.PaymentIsRequired)
                .SetValidator(new PaymentDtoValidator());
        }
    }
}