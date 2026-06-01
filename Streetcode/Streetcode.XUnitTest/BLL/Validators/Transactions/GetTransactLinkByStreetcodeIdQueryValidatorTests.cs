using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Transactions.TransactionLink.GetByStreetcodeId;
using Streetcode.BLL.Validators.Transactions.TransactionLink.GetByStreetcodeId;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Transactions.TransactionLink.GetByStreetcodeId
{
    public class GetTransactLinkByStreetcodeIdQueryValidatorTests
    {
        private readonly GetTransactLinkByStreetcodeIdQueryValidator _validator;

        public GetTransactLinkByStreetcodeIdQueryValidatorTests()
        {
            _validator = new GetTransactLinkByStreetcodeIdQueryValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_StreetcodeId_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new GetTransactLinkByStreetcodeIdQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.StreetcodeId);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_StreetcodeId_Is_Valid()
        {
            var query = new GetTransactLinkByStreetcodeIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}