using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Streetcode.Text.GetParsed;
using Streetcode.BLL.Validators.Streetcode.Text.GetParsed;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Streetcode.Text.GetParsed
{
    public class GetParsedTextForAdminPreviewCommandValidatorTests
    {
        private readonly GetParsedTextForAdminPreviewCommandValidator _validator;

        public GetParsedTextForAdminPreviewCommandValidatorTests()
        {
            _validator = new GetParsedTextForAdminPreviewCommandValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_TextToParse_Is_Empty(string invalidText)
        {
            var command = new GetParsedTextForAdminPreviewCommand(invalidText);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.textToParse)
                  .WithErrorMessage("Text to parse is required");
        }

        [Fact]
        public void Should_Not_Have_Errors_When_TextToParse_Is_Valid()
        {
            var command = new GetParsedTextForAdminPreviewCommand("Валідний текст для парсингу");

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}