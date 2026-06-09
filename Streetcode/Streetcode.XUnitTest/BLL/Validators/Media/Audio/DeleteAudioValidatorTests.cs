using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Media.Audio.Delete;
using Streetcode.BLL.Validators.Media.Audio.Delete;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Media.Audio.Delete
{
    public class DeleteAudioValidatorTests
    {
        private readonly DeleteAudioValidator _validator;

        public DeleteAudioValidatorTests()
        {
            _validator = new DeleteAudioValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-555)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var command = new DeleteAudioCommand(invalidId);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var command = new DeleteAudioCommand(1);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
