using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Media.Image.GetById;
using Streetcode.BLL.Validators.Media.Image.GetById;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Media.Image.GetById
{
    public class GetImageByIdValidatorTests
    {
        private readonly GetImageByIdValidator _validator;

        public GetImageByIdValidatorTests()
        {
            _validator = new GetImageByIdValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-555)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new GetImageByIdQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var query = new GetImageByIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
