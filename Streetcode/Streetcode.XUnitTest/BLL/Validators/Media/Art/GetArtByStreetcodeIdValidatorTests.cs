using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Media.Art.GetByStreetcodeId;
using Streetcode.BLL.Validators.Media.Art.GetByStreetcodeId;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Media.Art.GetByStreetcodeId
{
    public class GetArtByStreetcodeIdValidatorTests
    {
        private readonly GetArtByStreetcodeIdValidator _validator;

        public GetArtByStreetcodeIdValidatorTests()
        {
            _validator = new GetArtByStreetcodeIdValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-123)]
        public void Should_Have_Error_When_StreetcodeId_Is_Less_Or_Equal_To_Zero(int invalidStreetcodeId)
        {
            var query = new GetArtsByStreetcodeIdQuery(invalidStreetcodeId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_StreetcodeId_Is_Valid()
        {
            var query = new GetArtsByStreetcodeIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}