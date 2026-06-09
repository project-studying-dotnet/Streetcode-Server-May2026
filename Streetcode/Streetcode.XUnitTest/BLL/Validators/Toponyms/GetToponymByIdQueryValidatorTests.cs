using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Toponyms.GetById;
using Streetcode.BLL.Validators.Toponyms.GetById;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Toponyms.GetById
{
    public class GetToponymByIdQueryValidatorTests
    {
        private readonly GetToponymByIdQueryValidator _validator;

        public GetToponymByIdQueryValidatorTests()
        {
            _validator = new GetToponymByIdQueryValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new GetToponymByIdQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var query = new GetToponymByIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
