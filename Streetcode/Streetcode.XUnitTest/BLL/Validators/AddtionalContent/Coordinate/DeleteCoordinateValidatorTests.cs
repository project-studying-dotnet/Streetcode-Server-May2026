using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Delete;
using Streetcode.BLL.Validators.AdditionalContent.Coordinate.Delete;
using Xunit;

namespace Streetcode.XUnitTest.Validators.AdditionalContent.Coordinate.Delete
{
    public class DeleteCoordinateValidatorTests
    {
        private readonly DeleteCoordinateValidator _validator;

        public DeleteCoordinateValidatorTests()
        {
            _validator = new DeleteCoordinateValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        { 
            var command = new DeleteCoordinateCommand(invalidId);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {

            var command = new DeleteCoordinateCommand(1); 

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}