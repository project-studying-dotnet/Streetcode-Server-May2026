using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Interface;
using Streetcode.BLL.Validators;
using Streetcode.XUnitTest.BLL.Validators;
using Xunit;

namespace Streetcode.XUnitTest.Validators
{
    public class PositiveStreetcodeIdValidatorTests
    {
        private readonly TestPositiveStreetcodeIdValidator _validator;

        public PositiveStreetcodeIdValidatorTests()
        {
            _validator = new TestPositiveStreetcodeIdValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_StreetcodeId_Is_Not_Positive(int invalidId)
        {
            var query = new TestStreetcodeQuery { StreetcodeId = invalidId };

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeId)
                  .WithErrorMessage("StreetcodeId must be positive.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_StreetcodeId_Is_Positive()
        {
            var query = new TestStreetcodeQuery { StreetcodeId = 1 };

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}