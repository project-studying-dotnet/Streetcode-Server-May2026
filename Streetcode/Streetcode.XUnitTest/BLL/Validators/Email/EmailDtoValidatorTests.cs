using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Email;
using Streetcode.BLL.Validators.Email;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Email
{
    public class EmailDtoValidatorTests
    {
        private readonly EmailDtoValidator _validator;

        public EmailDtoValidatorTests()
        {
            _validator = new EmailDtoValidator();
        }

        [Fact]
        public void Should_Have_Error_When_From_Exceeds_Maximum_Length()
        {
            var longFrom = new string('A', 81);
            var dto = new EmailDTO { From = longFrom, Content = "Valid content" };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.From)
                  .WithErrorMessage("From must not exceed 80 characters");
        }

        [Fact]
        public void Should_Not_Have_Error_When_From_Is_At_Maximum_Length()
        {
            var maxFrom = new string('A', 80);
            var dto = new EmailDTO { From = maxFrom, Content = "Valid content" };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.From);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Content_Is_Empty_Or_Null(string? invalidContent)
        {
            var dto = new EmailDTO { From = "test@test.com", Content = invalidContent! };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Content)
                  .WithErrorMessage("Content is required");
        }

        [Fact]
        public void Should_Have_Error_When_Content_Exceeds_Maximum_Length()
        {
            var longContent = new string('B', 501);
            var dto = new EmailDTO { From = "test@test.com", Content = longContent };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Content)
                  .WithErrorMessage("Content must not exceed 500 characters");
        }

        [Theory]
        [InlineData("A")]
        [InlineData("Valid email body content within limits.")]
        public void Should_Not_Have_Errors_When_DTO_Is_Fully_Valid(string validContent)
        {
            var dto = new EmailDTO
            {
                From = "user@example.com",
                Content = validContent
            };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}