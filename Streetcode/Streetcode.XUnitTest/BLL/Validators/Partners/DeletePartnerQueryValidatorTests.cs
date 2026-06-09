using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Partners.Delete;
using Streetcode.BLL.Validators.Partners.Delete;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Partners.Delete
{
    public class DeletePartnerQueryValidatorTests
    {
        private readonly DeletePartnerQueryValidator _validator;

        public DeletePartnerQueryValidatorTests()
        {
            _validator = new DeletePartnerQueryValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-999)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new DeletePartnerQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var query = new DeletePartnerQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
