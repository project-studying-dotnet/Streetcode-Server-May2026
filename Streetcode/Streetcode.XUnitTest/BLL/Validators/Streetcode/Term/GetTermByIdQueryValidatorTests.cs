using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Streetcode.Term.GetById;
using Streetcode.BLL.Validators.Streetcode.Term.GetById;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Streetcode.Term.GetById
{
    public class GetTermByIdQueryValidatorTests
    {
        private readonly GetTermByIdQueryValidator _validator;

        public GetTermByIdQueryValidatorTests()
        {
            _validator = new GetTermByIdQueryValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new GetTermByIdQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var query = new GetTermByIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}