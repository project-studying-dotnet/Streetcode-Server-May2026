using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.Validators.AdditionalContent.Subtitle;
using Xunit;

namespace Streetcode.XUnitTest.Validators.AdditionalContent.Subtitle
{
    public class SubtitleDTOValidatorTests
    {
        private readonly SubtitleDTOValidator _validator;

        public SubtitleDTOValidatorTests()
        {
            _validator = new SubtitleDTOValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_SubtitleText_Is_Empty(string invalidText)
        {
            var dto = new SubtitleDTO { SubtitleText = invalidText, StreetcodeId = 1 };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.SubtitleText)
                  .WithErrorMessage("Subtitle text is required.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_SubtitleText_Is_Valid()
        {
            var dto = new SubtitleDTO { SubtitleText = "Some valid subtitle text", StreetcodeId = 1 };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.SubtitleText);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_StreetcodeId_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var dto = new SubtitleDTO { SubtitleText = "Valid Text", StreetcodeId = invalidId };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeId)
                  .WithErrorMessage("StreetcodeId must be greater than 0.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_StreetcodeId_Is_Greater_Than_Zero()
        {

            var dto = new SubtitleDTO { SubtitleText = "Valid Text", StreetcodeId = 5 };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.StreetcodeId);
        }

        [Fact]
        public void Should_Not_Have_Any_Validation_Errors_When_DTO_Is_Fully_Valid()
        {

            var dto = new SubtitleDTO
            {
                SubtitleText = "Просвіта — громадська організація, створена в Галичині.",
                StreetcodeId = 42
            };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}