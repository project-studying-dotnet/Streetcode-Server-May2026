using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Email;
using Streetcode.BLL.MediatR.Email;
using Streetcode.BLL.Validators.Email;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Email
{
    public class SendEmailCommandValidatorTests
    {
        private readonly SendEmailCommandValidator _validator;

        public SendEmailCommandValidatorTests()
        {
            _validator = new SendEmailCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Null()
        {
            var command = new SendEmailCommand(null!);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("Email payload is required");
        }

        [Fact]
        public void Should_Have_Error_When_Inner_Email_Content_Is_Empty()
        {
            var invalidEmailDto = new EmailDTO { From = "test@test.com", Content = "" };
            var command = new SendEmailCommand(invalidEmailDto);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Email.Content)
                  .WithErrorMessage("Content is required");
        }

        [Fact]
        public void Should_Not_Have_Any_Validation_Errors_When_Command_Is_Fully_Valid()
        {
            var validEmailDto = new EmailDTO { From = "user@example.com", Content = "Hello, this is a test message." };
            var command = new SendEmailCommand(validEmailDto);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}