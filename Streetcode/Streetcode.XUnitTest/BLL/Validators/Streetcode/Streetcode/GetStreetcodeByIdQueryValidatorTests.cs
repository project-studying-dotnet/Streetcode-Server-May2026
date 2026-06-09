using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetById;
using Streetcode.BLL.Validators.Streetcode.Streetcode.GetById;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Streetcode.Streetcode.GetById
{
    public class GetStreetcodeByIdQueryValidatorTests
    {
        private readonly GetStreetcodeByIdQueryValidator _validator;

        public GetStreetcodeByIdQueryValidatorTests()
        {
            _validator = new GetStreetcodeByIdQueryValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new GetStreetcodeByIdQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var query = new GetStreetcodeByIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
