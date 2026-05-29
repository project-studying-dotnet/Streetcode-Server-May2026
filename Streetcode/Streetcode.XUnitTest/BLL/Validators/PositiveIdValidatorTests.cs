using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Interface;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators;
using Streetcode.XUnitTest.BLL.Validators;
using Xunit;

namespace Streetcode.XUnitTest.Validators
{
    public class PositiveIdValidatorTests
    {
        private readonly TestPositiveIdValidator _validator;

        public PositiveIdValidatorTests()
        {
            _validator = new TestPositiveIdValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_Id_Is_Not_Positive(int invalidId)
        {
            var query = new TestQuery { Id = invalidId };

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Id)
                  .WithErrorMessage(ErrorMessages.IdMustBePositive);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Id_Is_Positive()
        {
            var query = new TestQuery { Id = 1 };

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}