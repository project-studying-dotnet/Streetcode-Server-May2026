using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Streetcode.RelatedFigure.GetByTagId;
using Streetcode.BLL.Validators.Streetcode.RelatedFigure.GetByTagId;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Streetcode.RelatedFigure.GetByTagId
{
    public class GetRelatedFiguresByTagIdQueryValidatorTests
    {
        private readonly GetRelatedFiguresByTagIdQueryValidator _validator;

        public GetRelatedFiguresByTagIdQueryValidatorTests()
        {
            _validator = new GetRelatedFiguresByTagIdQueryValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_TagId_Is_Less_Or_Equal_To_Zero(int invalidTagId)
        {
            var query = new GetRelatedFiguresByTagIdQuery(invalidTagId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.tagId)
                  .WithErrorMessage("The tagId must be positive.");
        }

        [Fact]
        public void Should_Not_Have_Errors_When_TagId_Is_Valid()
        {
            var query = new GetRelatedFiguresByTagIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}