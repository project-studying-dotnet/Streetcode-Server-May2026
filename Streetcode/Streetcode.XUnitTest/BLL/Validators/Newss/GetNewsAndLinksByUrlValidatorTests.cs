using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Newss.GetNewsAndLinksByUrl;
using Streetcode.BLL.Validators.Newss.GetNewsAndLinksByUrl;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Newss.GetNewsAndLinksByUrl
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
        public void Should_Have_Error_When_Url_Is_Empty(string invalidUrl)
        {
            var query = new GetNewsAndLinksByUrlQuery(invalidUrl);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.url);
        }

        [Theory]
        [InlineData("just-some-text")]
        [InlineData("www.google.com")] 
        [InlineData("/local/path/to/news")]
        [InlineData("http:relative-path")]
        public void Should_Have_Error_When_Url_Is_Not_A_Valid_Absolute_URL(string invalidUrl)
        {
            var query = new GetNewsAndLinksByUrlQuery(invalidUrl);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.url)
                  .WithErrorMessage("Url must be a valid absolute URL");
        }

        [Theory]
        [InlineData("https://streetcode.ua")]
        [InlineData("http://localhost:5001/news/1")]
        [InlineData("https://www.google.com/search?q=streetcode")]
        public void Should_Not_Have_Errors_When_Url_Is_Valid_Absolute_URL(string validUrl)
        {
            var query = new GetNewsAndLinksByUrlQuery(validUrl);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}