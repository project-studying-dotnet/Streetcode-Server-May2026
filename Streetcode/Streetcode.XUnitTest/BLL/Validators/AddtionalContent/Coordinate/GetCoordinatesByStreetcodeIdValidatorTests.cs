using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.GetByStreetcodeId;
using Streetcode.BLL.Validators.AdditionalContent.Coordinate.GetByStreetcodeId;
using Xunit;

namespace Streetcode.XUnitTest.Validators.AdditionalContent.Coordinate.GetByStreetcodeId
{
    public class GetCoordinatesByStreetcodeIdValidatorTests
    {
        private readonly GetCoordinatesByStreetcodeIdValidator _validator;

        public GetCoordinatesByStreetcodeIdValidatorTests()
        {
            _validator = new GetCoordinatesByStreetcodeIdValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-99)]
        public void Should_Have_Error_When_StreetcodeId_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new GetCoordinatesByStreetcodeIdQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_StreetcodeId_Is_Valid()
        {
            var query = new GetCoordinatesByStreetcodeIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}