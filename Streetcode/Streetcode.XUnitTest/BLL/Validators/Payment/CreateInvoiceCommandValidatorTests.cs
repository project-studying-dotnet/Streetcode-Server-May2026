using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Payment;
using Streetcode.BLL.MediatR.Payment;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Payment;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Payment
{
    public class CreateInvoiceCommandValidatorTests
    {
        private readonly CreateInvoiceCommandValidator _validator;

        public CreateInvoiceCommandValidatorTests()
        {
            _validator = new CreateInvoiceCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Payment_Is_Null()
        {
            var command = new CreateInvoiceCommand(null!);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Payment)
                  .WithErrorMessage(ErrorMessages.PaymentIsRequired);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Command_Is_Fully_Valid()
        {
            var validPaymentDto = new PaymentDTO
            {
                Amount = 100,
                RedirectUrl = "https://streetcode.ua"
            };
            var command = new CreateInvoiceCommand(validPaymentDto);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}