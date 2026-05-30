using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Partners.GetByStreetcodeId;
using Streetcode.BLL.Validators.Partners.GetByStreetcodeId;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Partners.GetByStreetcodeId
{
    public class GetPartnersByStreetcodeIdQueryValidatorTests
    {
        private readonly GetPartnersByStreetcodeIdQueryValidator _validator;

        public GetPartnersByStreetcodeIdQueryValidatorTests()
        {
            _validator = new GetPartnersByStreetcodeIdQueryValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-555)]
        public void Should_Have_Error_When_StreetcodeId_Is_Less_Or_Equal_To_Zero(int invalidStreetcodeId)
        {
            var query = new GetPartnersByStreetcodeIdQuery(invalidStreetcodeId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_StreetcodeId_Is_Valid()
        {
            var query = new GetPartnersByStreetcodeIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}