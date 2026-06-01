using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Media.Image.GetBaseImage;
using Streetcode.BLL.Validators.Media.Image.GetBaseImage;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Media.Image.GetBaseImage
{
    public class GetBaseImageValidatorTests
    {
        private readonly GetBaseImageValidator _validator;

        public GetBaseImageValidatorTests()
        {
            _validator = new GetBaseImageValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-456)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new GetBaseImageQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var query = new GetBaseImageQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}