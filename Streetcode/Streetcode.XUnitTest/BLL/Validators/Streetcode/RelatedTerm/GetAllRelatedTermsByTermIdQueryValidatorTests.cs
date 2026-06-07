using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Streetcode.RelatedTerm.GetAllByTermId;
using Streetcode.BLL.Validators.Streetcode.RelatedTerm.GetAllByTermId;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Streetcode.RelatedTerm.GetAllByTermId
{
    public class GetAllRelatedTermsByTermIdQueryValidatorTests
    {
        private readonly GetAllRelatedTermsByTermIdQueryValidator _validator;

        public GetAllRelatedTermsByTermIdQueryValidatorTests()
        {
            _validator = new GetAllRelatedTermsByTermIdQueryValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-50)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new GetAllRelatedTermsByTermIdQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var query = new GetAllRelatedTermsByTermIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
