using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Media.ArtSlides;
using Streetcode.BLL.MediatR.Media.ArtSlide.Create;
using Streetcode.BLL.Validators.Media.ArtSlides.Create;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Media.ArtSlides
{
    public class CreateArtSlideCommandValidatorTests
    {
        private readonly CreateArtSlideCommandValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Dto_Is_Null()
        {
            var command = new CreateArtSlideCommand(null!);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Dto);
        }

        [Fact]
        public void Should_Have_Error_When_Dto_Fields_Are_Invalid()
        {
            var invalidDto = new CreateStreetcodeArtSlideDto
            {
                Index = -5,
                TemplateId = 1,
                StreetcodeId = 1,
                ArtSlideItems = new List<ArtSlideItemDto>()
            };

            var command = new CreateArtSlideCommand(invalidDto);
            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Dto.Index);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Dto_Is_Valid()
        {
            var validDto = new CreateStreetcodeArtSlideDto
            {
                Index = 0,
                TemplateId = 1,
                StreetcodeId = 1,
                ArtSlideItems = new List<ArtSlideItemDto>()
            };

            var command = new CreateArtSlideCommand(validDto);
            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}