using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetByStreetcodeId;
using Streetcode.BLL.Validators.Streetcode.Fact.GetByStreetcodeId;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Streetcode.Fact
{
    public class GetFactByStreetcodeIdQueryValidatorTests
    {
        private readonly GetFactByStreetcodeIdQueryValidator _validator;

        public GetFactByStreetcodeIdQueryValidatorTests()
        {
            _validator = new GetFactByStreetcodeIdQueryValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-150)]
        public void Should_Have_Error_When_StreetcodeId_Is_Less_Or_Equal_To_Zero(int invalidStreetcodeId)
        {
            var query = new GetFactByStreetcodeIdQuery(invalidStreetcodeId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_StreetcodeId_Is_Valid()
        {
            var query = new GetFactByStreetcodeIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
