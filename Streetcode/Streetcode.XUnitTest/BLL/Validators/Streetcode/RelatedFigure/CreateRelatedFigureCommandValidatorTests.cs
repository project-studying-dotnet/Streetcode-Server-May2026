using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Streetcode.RelatedFigure.Create;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Streetcode.RelatedFigure.Сreate;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Streetcode.RelatedFigure.Create
{
    public class CreateRelatedFigureCommandValidatorTests
    {
        private readonly CreateRelatedFigureCommandValidator _validator;

        public CreateRelatedFigureCommandValidatorTests()
        {
            _validator = new CreateRelatedFigureCommandValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_ObserverId_Is_Less_Or_Equal_To_Zero(int invalidObserverId)
        {
            var command = new CreateRelatedFigureCommand(invalidObserverId, 1);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.ObserverId)
                  .WithErrorMessage(ErrorMessages.ObserverIdMustBePositive);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_TargetId_Is_Less_Or_Equal_To_Zero(int invalidTargetId)
        {
            var command = new CreateRelatedFigureCommand(1, invalidTargetId);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.TargetId)
                  .WithErrorMessage(ErrorMessages.TargetIdMustBePositive);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Command_Is_Fully_Valid()
        {
            var command = new CreateRelatedFigureCommand(1, 2);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
