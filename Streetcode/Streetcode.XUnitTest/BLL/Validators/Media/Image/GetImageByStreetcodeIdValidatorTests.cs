using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Media.Image.GetByStreetcodeId;
using Streetcode.BLL.Validators.Media.Image.GetByStreetcodeId;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Media.Image.GetByStreetcodeId
{
    public class GetImageByStreetcodeIdValidatorTests
    {
        private readonly GetImageByStreetcodeIdValidator _validator;

        public GetImageByStreetcodeIdValidatorTests()
        {
            _validator = new GetImageByStreetcodeIdValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_StreetcodeId_Is_Less_Or_Equal_To_Zero(int invalidStreetcodeId)
        {
            var query = new GetImageByStreetcodeIdQuery(invalidStreetcodeId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_StreetcodeId_Is_Valid()
        {
            var query = new GetImageByStreetcodeIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}