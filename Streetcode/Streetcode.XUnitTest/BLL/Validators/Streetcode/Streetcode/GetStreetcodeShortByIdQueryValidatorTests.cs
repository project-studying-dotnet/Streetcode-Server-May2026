using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetShortById;
using Streetcode.BLL.Validators.Streetcode.Streetcode.GetShortById;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Streetcode.Streetcode.GetShortById
{
    public class GetStreetcodeShortByIdQueryValidatorTests
    {
        private readonly GetStreetcodeShortByIdQueryValidator _validator;

        public GetStreetcodeShortByIdQueryValidatorTests()
        {
            _validator = new GetStreetcodeShortByIdQueryValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new GetStreetcodeShortByIdQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var query = new GetStreetcodeShortByIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
