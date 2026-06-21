using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Media.ArtSlides;
using Streetcode.BLL.MediatR.Media.ArtSlide.Update;
using Streetcode.BLL.Validators.Media.ArtSlides;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Media.ArtSlides.Update
{
    public class UpdateArtSlideCommandValidatorTests
    {
        private readonly UpdateArtSlideCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Dto_Is_Null()
        {
            var command = new UpdateArtSlideCommand(null!);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto);
        }

        [Fact]
        public void Should_Have_Error_When_Nested_Dto_Is_Invalid()
        {
            var command = new UpdateArtSlideCommand(new UpdateArtSlideDto { Id = 0 });
            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Dto.Id);
        }
    }
}