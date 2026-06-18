using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Media.Art.Delete;
using Streetcode.BLL.Validators.Media.Art.Delete;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Media.Art.Delete
{
    public class DeleteArtCommandValidatorTests
    {
        private readonly DeleteArtCommandValidator _validator;

        public DeleteArtCommandValidatorTests()
        {
            _validator = new DeleteArtCommandValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-999)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var command = new DeleteArtCommand(invalidId);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var command = new DeleteArtCommand(1);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}