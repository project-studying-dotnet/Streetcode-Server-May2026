using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.XUnitTest.Validators.Streetcode.Text;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Streetcode.Text
{
    public class TextCreateDtoValidatorTests
    {
        private readonly TextCreateDtoValidator _validator;

        public TextCreateDtoValidatorTests()
        {
            _validator = new TextCreateDtoValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Title_Is_Empty()
        {
            var model = new TextCreateDto { Title = "", TextContent = "Content" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Have_Error_When_TextContent_Is_Empty()
        {
            var model = new TextCreateDto { Title = "Title", TextContent = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.TextContent);
        }

        [Fact]
        public void Should_Have_Error_When_AdditionalText_Is_Too_Long()
        {
            var model = new TextCreateDto
            {
                Title = "Title",
                TextContent = "Content",
                AdditionalText = new string('a', 251)
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.AdditionalText);
        }

        [Fact]
        public void Should_Not_Have_Error_When_AdditionalText_Is_Valid()
        {
            var model = new TextCreateDto
            {
                Title = "Title",
                TextContent = "Content",
                AdditionalText = "Valid text"
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.AdditionalText);
        }

        [Fact]
        public void Should_Not_Have_Error_When_AdditionalText_Is_Null_Or_Whitespace()
        {
            var model = new TextCreateDto
            {
                Title = "Title",
                TextContent = "Content",
                AdditionalText = null
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.AdditionalText);
        }
    }
}
