using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetAllCatalog;
using Streetcode.BLL.Validators.Streetcode.Streetcode.GetAllCatalog;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Streetcode.Streetcode.GetAllCatalog
{
    public class GetAllStreetcodesCatalogQueryValidatorTests
    {
        private readonly GetAllStreetcodesCatalogQueryValidator _validator;

        public GetAllStreetcodesCatalogQueryValidatorTests()
        {
            _validator = new GetAllStreetcodesCatalogQueryValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void Should_Have_Error_When_Page_Is_Less_Or_Equal_To_Zero(int invalidPage)
        {
            var query = new GetAllStreetcodesCatalogQuery(invalidPage, 10);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.page)
                  .WithErrorMessage("The page number must be greater than 0.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void Should_Have_Error_When_Count_Is_Less_Or_Equal_To_Zero(int invalidCount)
        {
            var query = new GetAllStreetcodesCatalogQuery(1, invalidCount);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.count)
                  .WithErrorMessage("The page size (count) must be greater than 0.");
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Query_Is_Valid()
        {
            var query = new GetAllStreetcodesCatalogQuery(1, 10);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}