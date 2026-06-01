using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Sources.SourceLink.GetCategoryById;
using Streetcode.BLL.Validators.Sources.SourceLinkCategory.GetCategoryById;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Sources.SourceLinkCategory.GetCategoryById
{
    public class GetCategoryByIdQueryValidatorTests
    {
        private readonly GetCategoryByIdQueryValidator _validator;

        public GetCategoryByIdQueryValidatorTests()
        {
            _validator = new GetCategoryByIdQueryValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-99)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new GetCategoryByIdQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var query = new GetCategoryByIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}