using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Media.Audio.GetByStreetcodeId;
using Streetcode.BLL.Validators.Media.Audio.GetByStreetcodeId;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Media.Audio.GetByStreetcodeId
{
    public class GetAudioByStreetcodeIdValidatorTests
    {
        private readonly GetAudioByStreetcodeIdValidator _validator;

        public GetAudioByStreetcodeIdValidatorTests()
        {
            _validator = new GetAudioByStreetcodeIdValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-777)]
        public void Should_Have_Error_When_StreetcodeId_Is_Less_Or_Equal_To_Zero(int invalidStreetcodeId)
        {
            var query = new GetAudioByStreetcodeIdQuery(invalidStreetcodeId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_StreetcodeId_Is_Valid()
        {
            var query = new GetAudioByStreetcodeIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
