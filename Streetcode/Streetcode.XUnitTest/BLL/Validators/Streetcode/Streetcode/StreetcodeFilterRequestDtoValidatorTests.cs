using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.AdditionalContent.Filter;
using Streetcode.BLL.Validators.Streetcode.Streetcode.GetByFilter;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Streetcode.Streetcode.GetByFilter
{
    public class StreetcodeFilterRequestDtoValidatorTests
    {
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
                  .WithErrorMessage("Search Query must be 255 characters or less.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Have_Error_When_SearchQuery_Is_Empty(string invalidQuery)
        {
            var dto = new StreetcodeFilterRequestDTO { SearchQuery = invalidQuery };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.SearchQuery)
                  .WithErrorMessage("Search Query is required.");
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