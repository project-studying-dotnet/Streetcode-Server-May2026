using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Media.Audio.GetBaseAudio;
using Streetcode.BLL.Validators.Media.Audio.GetBase;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Media.Audio.GetBase
{
    public class GetBaseAudioValidatorTests
    {
        private readonly GetBaseAudioValidator _validator;

        public GetBaseAudioValidatorTests()
        {
            _validator = new GetBaseAudioValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new GetBaseAudioQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var query = new GetBaseAudioQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}