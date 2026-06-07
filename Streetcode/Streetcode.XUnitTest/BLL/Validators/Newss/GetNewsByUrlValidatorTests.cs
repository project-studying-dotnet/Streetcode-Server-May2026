using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Newss.GetByUrl;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Newss.GetByUrl;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Newss
{
    public class GetNewsByUrlValidatorTests
    {
        private readonly GetNewsByUrlValidator _validator;

        public GetNewsByUrlValidatorTests()
        {
            _validator = new GetNewsByUrlValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_Url_Is_Empty(string? invalidUrl)
        {
            var query = new GetNewsByUrlQuery(invalidUrl!);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Url)
                  .WithErrorMessage(ErrorMessages.UrlIsRequired);
        }

        [Fact]
        public void Should_Have_Error_When_Url_Exceeds_Max_Length()
        {
            var longUrl = new string('a', 2049);
            var query = new GetNewsByUrlQuery(longUrl);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Url)
                  .WithErrorMessage(string.Format(ErrorMessages.UrlMustNotExceedCharacters, 2048));
        }

        [Theory]
        [InlineData("valid-news-slug")]
        [InlineData("news-item-123")]
        public void Should_Not_Have_Errors_When_Url_Is_Valid(string validUrl)
        {
            var query = new GetNewsByUrlQuery(validUrl);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}