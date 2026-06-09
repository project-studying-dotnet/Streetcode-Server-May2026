using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Streetcode.Text.GetById;
using Streetcode.BLL.Validators.Streetcode.Text.GetById;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Streetcode.Text.GetById
{
    public class GetTextByIdQueryValidatorTests
    {
        private readonly GetTextByIdQueryValidator _validator;

        public GetTextByIdQueryValidatorTests()
        {
            _validator = new GetTextByIdQueryValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new GetTextByIdQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var query = new GetTextByIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
