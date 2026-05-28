using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.AdditionalContent.GetById;
using Streetcode.BLL.Validators.AdditionalContent.Subtitle.GetById;
using Xunit;

namespace Streetcode.XUnitTest.Validators.AdditionalContent.Subtitle.GetById
{
    public class GetSubtitleByIdValidatorTests
    {
        private readonly GetSubtitleByIdValidator _validator;

        public GetSubtitleByIdValidatorTests()
        {
            _validator = new GetSubtitleByIdValidator();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-5)]
        public void Should_Have_Error_When_Id_Is_Less_Or_Equal_To_Zero(int invalidId)
        {
            var query = new GetSubtitleByIdQuery(invalidId);

            var result = _validator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var query = new GetSubtitleByIdQuery(1);

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}