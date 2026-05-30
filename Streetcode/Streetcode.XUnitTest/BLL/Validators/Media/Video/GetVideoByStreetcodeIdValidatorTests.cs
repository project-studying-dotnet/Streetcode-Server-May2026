using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Media.Video.GetByStreetcodeId;
using Streetcode.BLL.Validators.Media.Video.GetByStreetcodeId;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Media.Video.GetByStreetcodeId
{
    public class GetVideoByStreetcodeIdValidatorTests
    {
        private readonly GetVideoByStreetcodeIdValidator _validator;

        public GetVideoByStreetcodeIdValidatorTests()
        {
            _validator = new GetVideoByStreetcodeIdValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-99)]
        public void Should_Have_Error_When_StreetcodeId_Is_Less_Or_Equal_To_Zero(int invalidStreetcodeId)
        {
            var query = new GetVideoByStreetcodeIdQuery(invalidStreetcodeId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_StreetcodeId_Is_Valid()
        {
            var query = new GetVideoByStreetcodeIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}