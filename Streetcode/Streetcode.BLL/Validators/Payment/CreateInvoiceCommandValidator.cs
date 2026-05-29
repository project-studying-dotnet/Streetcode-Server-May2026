using FluentValidation;
using Streetcode.BLL.MediatR.Payment;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Payment
{
    public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
    {
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