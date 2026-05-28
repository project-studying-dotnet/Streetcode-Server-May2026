using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.AdditionalContent.Filter;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByFilter;
using Streetcode.BLL.Validators.Streetcode.Streetcode.GetByFilter;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Streetcode.Streetcode.GetByFilter
{
    public class GetStreetcodeByFilterQueryValidatorTests
    {
        private readonly GetStreetcodeByFilterQueryValidator _validator;

        public GetStreetcodeByFilterQueryValidatorTests()
        {
            _validator = new GetStreetcodeByFilterQueryValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Filter_Is_Null()
        {
            var query = new GetStreetcodeByFilterQuery(null!);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Filter)
                  .WithErrorMessage("Filter is required");
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Filter_Is_Valid()
        {
            var validFilter = new StreetcodeFilterRequestDTO
            {
                SearchQuery = "Валідний запит" 
            };

            var query = new GetStreetcodeByFilterQuery(validFilter);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}