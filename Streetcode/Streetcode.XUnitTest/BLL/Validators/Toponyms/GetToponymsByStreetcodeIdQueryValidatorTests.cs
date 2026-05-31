using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Toponyms.GetByStreetcodeId;
using Streetcode.BLL.Validators.Toponyms.GetByStreetcodeId;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Toponyms.GetByStreetcodeId
{
    public class GetToponymsByStreetcodeIdQueryValidatorTests
    {
        private readonly GetToponymsByStreetcodeIdQueryValidator _validator;

        public GetToponymsByStreetcodeIdQueryValidatorTests()
        {
            _validator = new GetToponymsByStreetcodeIdQueryValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_StreetcodeId_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new GetToponymsByStreetcodeIdQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_StreetcodeId_Is_Valid()
        {
            var query = new GetToponymsByStreetcodeIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}