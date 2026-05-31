using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoriesByStreetcodeId;
using Streetcode.BLL.Validators.Sources.SourceLinkCategory.GetCategoriesByStreetcodeId;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Sources.SourceLinkCategory.GetCategoriesByStreetcodeId
{
    public class GetCategoriesByStreetcodeIdQueryValidatorTests
    {
        private readonly GetCategoriesByStreetcodeIdQueryValidator _validator;

        public GetCategoriesByStreetcodeIdQueryValidatorTests()
        {
            _validator = new GetCategoriesByStreetcodeIdQueryValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-123)]
        public void Should_Have_Error_When_StreetcodeId_Is_Less_Or_Equal_To_Zero(int invalidStreetcodeId)
        {
            var query = new GetCategoriesByStreetcodeIdQuery(invalidStreetcodeId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_StreetcodeId_Is_Valid()
        {
            var query = new GetCategoriesByStreetcodeIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}