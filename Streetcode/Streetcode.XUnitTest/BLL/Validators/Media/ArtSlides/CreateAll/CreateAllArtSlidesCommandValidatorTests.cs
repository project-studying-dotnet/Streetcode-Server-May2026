using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Media.ArtSlides;
using Streetcode.BLL.MediatR.Media.ArtSlide.CreateAll;
using Streetcode.BLL.Validators.Media.ArtSlides;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Media.ArtSlides
{
    public class CreateAllArtSlidesCommandValidatorTests
    {
        private readonly CreateAllArtSlidesCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_ArtSlides_Is_Null_Or_Empty()
        {
            var commandNull = new CreateAllArtSlidesCommand(null!);
            var commandEmpty = new CreateAllArtSlidesCommand(new List<CreateStreetcodeArtSlideDto>());

            var resultNull = _validator.TestValidate(commandNull);
            var resultEmpty = _validator.TestValidate(commandEmpty);

            resultNull.ShouldHaveValidationErrorFor(x => x.ArtSlides);
            resultEmpty.ShouldHaveValidationErrorFor(x => x.ArtSlides);
        }

        [Fact]
        public void Should_Have_Error_When_List_Contains_Invalid_Item()
        {
            var command = new CreateAllArtSlidesCommand(new List<CreateStreetcodeArtSlideDto>
            {
                new() { Index = -1 }
            });

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor("ArtSlides[0].Index");
        }
    }
}