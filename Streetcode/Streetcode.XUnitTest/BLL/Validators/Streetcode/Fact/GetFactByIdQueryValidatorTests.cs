using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Streetcode.Fact.GetById;
using Streetcode.BLL.Validators.Streetcode.Fact.GetById;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Streetcode.Fact
{
    public class GetFactByIdQueryValidatorTests
    {
        private readonly GetFactByIdQueryValidator _validator;

        public GetFactByIdQueryValidatorTests()
        {
            _validator = new GetFactByIdQueryValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-999)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new GetFactByIdQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var query = new GetFactByIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}