using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.GetById;
using Streetcode.BLL.Validators.AdditionalContent.Tag.GetById;
using Xunit;

namespace Streetcode.XUnitTest.Validators.AdditionalContent.Tag.GetById
{
    public class GetTagByIdValidatorTests
    {
        private readonly GetTagByIdValidator _validator;

        public GetTagByIdValidatorTests()
        {
            _validator = new GetTagByIdValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new GetTagByIdQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var query = new GetTagByIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}