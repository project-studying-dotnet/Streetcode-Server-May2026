using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.AdditionalContent.Tag.Create;
using Xunit;

namespace Streetcode.XUnitTest.Validators.AdditionalContent.Tag.Create
{
    public class CreateTagDtoValidatorTests
    {
        private readonly CreateTagDtoValidator _validator;

        public CreateTagDtoValidatorTests()
        {
            _validator = new CreateTagDtoValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Should_Have_Error_When_Title_Is_Empty_Or_Null(string? invalidTitle)
        {
            var dto = new CreateTagDTO { Title = invalidTitle! };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Title)
                    .WithErrorMessage(ErrorMessages.TitleIsRequired);
        }

        [Theory]
        [InlineData("   ")]
        [InlineData("\n")]
        [InlineData("\t")]
        public void Should_Have_Error_When_Title_Contains_Only_Whitespace(string whitespaceTitle)
        {
            var dto = new CreateTagDTO { Title = whitespaceTitle };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Title)
                    .WithErrorMessage(ErrorMessages.TitleIsRequired);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Title_Is_Valid()
        {
            var dto = new CreateTagDTO { Title = "Історія" };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}