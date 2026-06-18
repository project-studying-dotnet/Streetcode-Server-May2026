using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Media.ArtSlide.Delete;
using Streetcode.BLL.Validators.Media.ArtSlides.Delete;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Media.ArtSlides.Delete
{
    public class DeleteArtSlideCommandValidatorTests
    {
        private readonly DeleteArtSlideCommandValidator _validator = new();

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Should_Have_Error_When_Id_Is_Not_Positive(int invalidId)
        {
            var command = new DeleteArtSlideCommand(invalidId);
            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Id_Is_Valid()
        {
            var command = new DeleteArtSlideCommand(1);
            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}