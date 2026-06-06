using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.DeleteSoft;
using Streetcode.BLL.Validators.Streetcode.Streetcode.DeleteSoft;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Streetcode.Streetcode.DeleteSoft
{
    public class DeleteSoftStreetcodeCommandValidatorTests
    {
        private readonly DeleteSoftStreetcodeCommandValidator _validator;

        public DeleteSoftStreetcodeCommandValidatorTests()
        {
            _validator = new DeleteSoftStreetcodeCommandValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var command = new DeleteSoftStreetcodeCommand(invalidId);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var command = new DeleteSoftStreetcodeCommand(1);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
