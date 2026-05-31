using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Media.Image.Delete;
using Streetcode.BLL.Validators.Media.Image.Delete;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Media.Image.Delete
{
    public class DeleteImageValidatorTests
    {
        private readonly DeleteImageValidator _validator;

        public DeleteImageValidatorTests()
        {
            _validator = new DeleteImageValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-999)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var command = new DeleteImageCommand(invalidId);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var command = new DeleteImageCommand(1);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}