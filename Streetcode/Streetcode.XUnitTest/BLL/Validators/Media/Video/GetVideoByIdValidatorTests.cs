using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Media.Video.GetById;
using Streetcode.BLL.Validators.Video.Image.GetById;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Video.Image.GetById
{
    public class GetVideoByIdValidatorTests
    {
        private readonly GetVideoByIdValidator _validator;

        public GetVideoByIdValidatorTests()
        {
            _validator = new GetVideoByIdValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-777)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new GetVideoByIdQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var query = new GetVideoByIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}