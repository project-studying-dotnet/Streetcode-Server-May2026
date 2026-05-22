using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByTransliterationUrl;
using Streetcode.BLL.Validators.Streetcode.Streetcode.GetByTransliterationUrl;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Streetcode.Streetcode.GetByTransliterationUrl
{
    public class GetStreetcodeByTransliterationUrlQueryValidatorTests
    {
        private readonly GetStreetcodeByTransliterationUrlQueryValidator _validator;

        public GetStreetcodeByTransliterationUrlQueryValidatorTests()
        {
            _validator = new GetStreetcodeByTransliterationUrlQueryValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_Url_Is_Empty(string invalidUrl)
        {
            var query = new GetStreetcodeByTransliterationUrlQuery(invalidUrl);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.url)
                  .WithErrorMessage("Transliteration url is required");
        }

        [Fact]
        public void Should_Have_Error_When_Url_Exceeds_255_Characters()
        {
            var invalidUrl = new string('a', 256);
            var query = new GetStreetcodeByTransliterationUrlQuery(invalidUrl);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.url)
                  .WithErrorMessage("Transliteration url must not exceed 255 characters");
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Url_Is_Valid()
        {
            var validUrl = "valid-transliteration-url";
            var query = new GetStreetcodeByTransliterationUrlQuery(validUrl);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Url_Is_Exactly_255_Characters()
        {
            var validUrl = new string('a', 255);
            var query = new GetStreetcodeByTransliterationUrlQuery(validUrl);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}