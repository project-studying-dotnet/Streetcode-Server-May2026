using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.Validators.Media.Art;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Validators.Media.Art
{
    public class ArtUpdateDtoValidatorTests
    {
        private readonly ArtUpdateDtoValidator _validator = new();

        [Fact]
        public void DtoValidator_Should_Detect_Errors_When_Fields_Are_Invalid()
        {
            var model = new ArtUpdateDto { Id = 0, ImageId = 0, Title = new string('a', 151) };
            var result = _validator.TestValidate(model);

            result.ShouldHaveValidationErrorFor(x => x.Id);
            result.ShouldHaveValidationErrorFor(x => x.ImageId);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void DtoValidator_Should_Pass_When_Data_Is_Valid()
        {
            var model = new ArtUpdateDto { Id = 1, ImageId = 1, Title = "Valid" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}