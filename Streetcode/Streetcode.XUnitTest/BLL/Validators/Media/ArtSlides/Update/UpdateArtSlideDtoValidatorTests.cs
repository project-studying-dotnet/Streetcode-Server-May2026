using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Media.ArtSlides;
using Streetcode.BLL.Validators.Media.ArtSlides.Update;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Media.ArtSlides.Update
{
    public class UpdateArtSlideDtoValidatorTests
    {
        private readonly UpdateArtSlideDtoValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Id_Is_Zero()
        {
            var model = new UpdateArtSlideDto { Id = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Data_Is_Valid()
        {
            var model = new UpdateArtSlideDto
            {
                Id = 1,
                Index = 1,
                TemplateId = 1,
                StreetcodeId = 1
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}