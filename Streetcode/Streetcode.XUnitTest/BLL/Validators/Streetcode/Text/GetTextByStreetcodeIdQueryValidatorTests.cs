using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Streetcode.Text.GetByStreetcodeId;
using Streetcode.BLL.Validators.Streetcode.Text.GetByStreetcodeId;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Streetcode.Text.GetByStreetcodeId
{
    public class GetTextByStreetcodeIdQueryValidatorTests
    {
        private readonly GetTextByStreetcodeIdQueryValidator _validator;

        public GetTextByStreetcodeIdQueryValidatorTests()
        {
            _validator = new GetTextByStreetcodeIdQueryValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_StreetcodeId_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new GetTextByStreetcodeIdQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_StreetcodeId_Is_Valid()
        {
            var query = new GetTextByStreetcodeIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}