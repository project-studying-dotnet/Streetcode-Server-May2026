using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByIndex;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Streetcode.Streetcode.GetByIndex;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Streetcode.Streetcode.GetByIndex
{
    public class GetStreetcodeByIndexQueryValidatorTests
    {
        private readonly GetStreetcodeByIndexQueryValidator _validator;

        public GetStreetcodeByIndexQueryValidatorTests()
        {
            _validator = new GetStreetcodeByIndexQueryValidator();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-50)]
        public void Should_Have_Error_When_Index_Is_Less_Than_Zero(int invalidIndex)
        {
            var query = new GetStreetcodeByIndexQuery(invalidIndex);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Index)
                  .WithErrorMessage(ErrorMessages.IndexMustBeGreaterOrEqualToZero);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public void Should_Not_Have_Errors_When_Index_Is_Greater_Or_Equal_To_Zero(int validIndex)
        {
            var query = new GetStreetcodeByIndexQuery(validIndex);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
