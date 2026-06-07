using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.AdditionalContent.Filter;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Streetcode.Streetcode.GetByFilter;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Streetcode.Streetcode.GetByFilter
{
    public class StreetcodeFilterRequestDtoValidatorTests
    {
        private const int MaxSearchQueryLength = 255;
        private readonly StreetcodeFilterRequestDtoValidator _validator;

        public StreetcodeFilterRequestDtoValidatorTests()
        {
            _validator = new StreetcodeFilterRequestDtoValidator();
        }

        [Fact]
        public void Should_Have_Error_When_SearchQuery_Exceeds_255_Characters()
        {
            var dto = new StreetcodeFilterRequestDTO
            {
                SearchQuery = new string('a', 256)
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.SearchQuery)
                  .WithErrorMessage(string.Format(ErrorMessages.SearchQueryMustNotExceedCharacters, MaxSearchQueryLength));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_SearchQuery_Is_Empty(string? invalidQuery)
        {
            var dto = new StreetcodeFilterRequestDTO { SearchQuery = invalidQuery! };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.SearchQuery)
                  .WithErrorMessage(ErrorMessages.SearchQueryIsRequired);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_SearchQuery_Is_Valid()
        {
            var dto = new StreetcodeFilterRequestDTO
            {
                SearchQuery = "Валідний запит"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Errors_When_SearchQuery_Is_Exactly_255_Characters()
        {
            var dto = new StreetcodeFilterRequestDTO
            {
                SearchQuery = new string('a', 255)
            };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
