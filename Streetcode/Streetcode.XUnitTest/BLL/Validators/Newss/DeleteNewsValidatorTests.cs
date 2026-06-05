using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Newss.Delete;
using Streetcode.BLL.Validators.Newss.Delete;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Newss.Delete
{
    public class DeleteNewsValidatorTests
    {
        private readonly DeleteNewsValidator _validator;

        public DeleteNewsValidatorTests()
        {
            _validator = new DeleteNewsValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-1234)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var command = new DeleteNewsCommand(invalidId);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var command = new DeleteNewsCommand(1);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
