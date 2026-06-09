using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Streetcode.RelatedFigure.Delete;
using Streetcode.BLL.Validators.Streetcode.RelatedFigure.Delete;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Streetcode.RelatedFigure.Delete
{
    public class DeleteRelatedFigureCommandValidatorTests
    {
        private readonly DeleteRelatedFigureCommandValidator _validator;

        public DeleteRelatedFigureCommandValidatorTests()
        {
            _validator = new DeleteRelatedFigureCommandValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-99)]
        public void Should_Have_Error_When_ObserverId_Is_Less_Or_Equal_To_Zero(int invalidObserverId)
        {
            var command = new DeleteRelatedFigureCommand(invalidObserverId, 1);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.ObserverId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-50)]
        public void Should_Have_Error_When_TargetId_Is_Less_Or_Equal_To_Zero(int invalidTargetId)
        {
            var command = new DeleteRelatedFigureCommand(1, invalidTargetId);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.TargetId);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Command_Is_Fully_Valid()
        {
            var command = new DeleteRelatedFigureCommand(1, 2);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
