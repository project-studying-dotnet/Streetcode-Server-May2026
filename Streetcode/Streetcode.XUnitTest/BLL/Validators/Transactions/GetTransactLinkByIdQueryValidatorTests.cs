using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Transactions.TransactionLink.GetById;
using Streetcode.BLL.Validators.Transactions.TransactionLink.GetById;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Transactions.TransactionLink.GetById
{
    public class GetTransactLinkByIdQueryValidatorTests
    {
        private readonly GetTransactLinkByIdQueryValidator _validator;

        public GetTransactLinkByIdQueryValidatorTests()
        {
            _validator = new GetTransactLinkByIdQueryValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new GetTransactLinkByIdQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var query = new GetTransactLinkByIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
