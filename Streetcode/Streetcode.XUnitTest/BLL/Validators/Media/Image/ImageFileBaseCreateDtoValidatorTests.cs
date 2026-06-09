using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.Validators.Media.Image.Create;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Media.Image.Create
{
    public class ImageFileBaseCreateDtoValidatorTests
    {
        private readonly ImageFileBaseCreateDtoValidator _validator;

        public ImageFileBaseCreateDtoValidatorTests()
        {
            _validator = new ImageFileBaseCreateDtoValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Alt_Exceeds_Maximum_Length()
        {
            var longAlt = new string('A', 256);
            var dto = CreateValidImageDto(longAlt);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Alt);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Alt_Is_Within_Allowed_Length()
        {
            var validAlt = new string('A', 255);
            var dto = CreateValidImageDto(validAlt);

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Alt);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Valid descriptive alt text")]
        public void Should_Not_Have_Errors_When_DTO_Is_Valid(string? altText)
        {
            var dto = CreateValidImageDto(altText!);

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private static ImageFileBaseCreateDTO CreateValidImageDto(string alt)
        {
            return new ImageFileBaseCreateDTO
            {
                Alt = alt,
                Title = "valid_image_title",
                BaseFormat = "png",
                MimeType = "image/png",
                Extension = ".png"
            };
        }
    }
}
