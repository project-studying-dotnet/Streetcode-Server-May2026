using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Media.ArtSlides;
using Streetcode.BLL.Validators.Media.ArtSlides.Create;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Media.ArtSlides
{
    public class CreateStreetcodeArtSlideDtoValidatorTests
    {
        private readonly CreateStreetcodeArtSlideDtoValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Index_Is_Negative()
        {
            var model = new CreateStreetcodeArtSlideDto { Index = -1 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Index);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Data_Is_Valid()
        {
            var model = new CreateStreetcodeArtSlideDto
            {
                Index = 1,
                TemplateId = 1,
                StreetcodeId = 1,
                ArtSlideItems = new List<ArtSlideItemDto>()
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}