using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.AdditionalContent.Subtitle.GetByStreetcodeId;
using Streetcode.BLL.Validators.AdditionalContent.Subtitle.GetById;
using Xunit;

namespace Streetcode.XUnitTest.Validators.AdditionalContent.Subtitle.GetById
{
    public class GetSubtitleByStreetcodeIdValidatorTests
    {
        private readonly GetSubtitleByStreetcodeIdValidator _validator;

        public GetSubtitleByStreetcodeIdValidatorTests()
        {
            _validator = new GetSubtitleByStreetcodeIdValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-15)]
        public void Should_Have_Error_When_StreetcodeId_Is_Less_Or_Equal_To_Zero(int invalidStreetcodeId)
        {
            var query = new GetSubtitlesByStreetcodeIdQuery(invalidStreetcodeId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_StreetcodeId_Is_Valid()
        {
            var query = new GetSubtitlesByStreetcodeIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}