using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Partners.GetById;
using Streetcode.BLL.Validators.Partners.GetById;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Partners.GetById
{
    public class GetPartnerByIdQueryValidatorTests
    {
        private readonly GetPartnerByIdQueryValidator _validator;

        public GetPartnerByIdQueryValidatorTests()
        {
            _validator = new GetPartnerByIdQueryValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new GetPartnerByIdQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var query = new GetPartnerByIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}