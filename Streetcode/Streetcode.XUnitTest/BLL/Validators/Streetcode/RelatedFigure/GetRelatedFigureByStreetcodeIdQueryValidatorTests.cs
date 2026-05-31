using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Streetcode.RelatedFigure.GetByStreetcodeId;
using Streetcode.BLL.Validators.Streetcode.RelatedFigure.GetByStreetcodeId;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Streetcode.RelatedFigure.GetByStreetcodeId
{
    public class GetRelatedFigureByStreetcodeIdQueryValidatorTests
    {
        private readonly GetRelatedFigureByStreetcodeIdQueryValidator _validator;

        public GetRelatedFigureByStreetcodeIdQueryValidatorTests()
        {
            _validator = new GetRelatedFigureByStreetcodeIdQueryValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-250)]
        public void Should_Have_Error_When_StreetcodeId_Is_Less_Or_Equal_To_Zero(int invalidStreetcodeId)
        {
            var query = new GetRelatedFigureByStreetcodeIdQuery(invalidStreetcodeId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_StreetcodeId_Is_Valid()
        {
            var query = new GetRelatedFigureByStreetcodeIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}