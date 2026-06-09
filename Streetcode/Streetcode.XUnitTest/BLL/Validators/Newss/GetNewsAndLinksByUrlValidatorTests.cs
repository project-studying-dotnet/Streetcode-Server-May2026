using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Newss.GetNewsAndLinksByUrl;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Newss.GetNewsAndLinksByUrl;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Newss
{
    public class GetNewsAndLinksByUrlValidatorTests
    {
        private readonly GetNewsAndLinksByUrlValidator _validator;

        public GetNewsAndLinksByUrlValidatorTests()
        {
            _validator = new GetNewsAndLinksByUrlValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_Url_Is_Empty(string? invalidUrl)
        {
            var query = new GetNewsAndLinksByUrlQuery(invalidUrl!);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Url)
                  .WithErrorMessage(ErrorMessages.UrlIsRequired);
        }

        [Fact]
        public void Should_Have_Error_When_Url_Exceeds_Max_Length()
        {
            var longUrl = new string('a', 2049);
            var query = new GetNewsAndLinksByUrlQuery(longUrl);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Url)
                  .WithErrorMessage(string.Format(ErrorMessages.UrlMustNotExceedCharacters, 2048));
        }

        [Theory]
        [InlineData("lol")]
        [InlineData("some-valid-news-slug")]
        [InlineData("https://streetcode.ua")]
        public void Should_Not_Have_Errors_When_Url_Is_Valid(string validUrl)
        {
            var query = new GetNewsAndLinksByUrlQuery(validUrl);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
