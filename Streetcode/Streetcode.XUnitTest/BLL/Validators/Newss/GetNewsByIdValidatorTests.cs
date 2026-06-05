using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Newss.GetById;
using Streetcode.BLL.Validators.Newss.GetById;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Newss
{
    public class GetNewsByIdValidatorTests
    {
        private readonly GetNewsByIdValidator _validator;

        public GetNewsByIdValidatorTests()
        {
            _validator = new GetNewsByIdValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-999)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new GetNewsByIdQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var query = new GetNewsByIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}