using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Media.Audio.GetById;
using Streetcode.BLL.Validators.Media.Audio.GetById;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Media.Audio.GetById
{
    public class GetAudioByIdValidatorTests
    {
        private readonly GetAudioByIdValidator _validator;

        public GetAudioByIdValidatorTests()
        {
            _validator = new GetAudioByIdValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-1234)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new GetAudioByIdQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var query = new GetAudioByIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}