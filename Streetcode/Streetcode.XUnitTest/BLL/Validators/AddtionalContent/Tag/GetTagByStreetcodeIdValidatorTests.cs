using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetByStreetcodeId;
using Streetcode.BLL.Validators.AdditionalContent.Tag.GetByStreetcodeId;
using Xunit;

namespace Streetcode.XUnitTest.Validators.AdditionalContent.Tag.GetByStreetcodeId
{
    public class GetTagByStreetcodeIdValidatorTests
    {
        private readonly GetTagByStreetcodeIdValidator _validator;

        public GetTagByStreetcodeIdValidatorTests()
        {
            _validator = new GetTagByStreetcodeIdValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-50)]
        public void Should_Have_Error_When_StreetcodeId_Is_Less_Or_Equal_To_Zero(int invalidStreetcodeId)
        {
            var query = new GetTagByStreetcodeIdQuery(invalidStreetcodeId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_StreetcodeId_Is_Valid()
        {
            var query = new GetTagByStreetcodeIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}