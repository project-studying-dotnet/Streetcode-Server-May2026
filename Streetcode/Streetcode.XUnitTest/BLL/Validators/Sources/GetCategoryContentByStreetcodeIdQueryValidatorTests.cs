using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.GetCategoryContentByStreetcodeId;
using Streetcode.BLL.Validators.Sources.SourceLinkCategory.GetCategoryContentByStreetcodeId;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Sources.SourceLinkCategory.GetCategoryContentByStreetcodeId
{
    public class GetCategoryContentByStreetcodeIdQueryValidatorTests
    {
        private readonly GetCategoryContentByStreetcodeIdQueryValidator _validator;

        public GetCategoryContentByStreetcodeIdQueryValidatorTests()
        {
            _validator = new GetCategoryContentByStreetcodeIdQueryValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_StreetcodeId_Is_Less_Or_Equal_To_Zero(int invalidStreetcodeId)
        {
            var query = new GetCategoryContentByStreetcodeIdQuery(invalidStreetcodeId, 1);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.streetcodeId)
                  .WithErrorMessage("The streetcodeId must be positive.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-50)]
        public void Should_Have_Error_When_CategoryId_Is_Less_Or_Equal_To_Zero(int invalidCategoryId)
        {
            var query = new GetCategoryContentByStreetcodeIdQuery(1, invalidCategoryId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.categoryId)
                  .WithErrorMessage("The categoryId must be positive.");
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Query_Is_Fully_Valid()
        {
            var query = new GetCategoryContentByStreetcodeIdQuery(1, 1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}