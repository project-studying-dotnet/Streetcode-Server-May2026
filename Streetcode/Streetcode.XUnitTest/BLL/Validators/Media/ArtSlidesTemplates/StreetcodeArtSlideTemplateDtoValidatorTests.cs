using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Media.ArtSlidesTemplates;
using Streetcode.BLL.Validators.Media.ArtSlidesTemplates;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Media.ArtSlidesTemplates
{
    public class StreetcodeArtSlideTemplateDtoValidatorTests
    {
        private readonly StreetcodeArtSlideTemplateDtoValidator _validator = new();

        [Fact]
        public void Should_Have_Error_When_Id_Is_Zero_Or_Negative()
        {
            var model = new StreetcodeArtSlideTemplateDto { Id = 0, Name = "Valid Name" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            var model = new StreetcodeArtSlideTemplateDto { Id = 1, Name = "" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Exceeds_Max_Length()
        {
            var model = new StreetcodeArtSlideTemplateDto { Id = 1, Name = new string('a', 256) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Not_Have_Errors_When_Data_Is_Valid()
        {
            var model = new StreetcodeArtSlideTemplateDto { Id = 1, Name = "Valid Template Name" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}