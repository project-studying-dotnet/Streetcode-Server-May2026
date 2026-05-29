using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.MediatR.Media.Image.Create;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Media.Image.Create;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Media.Image.Create
{
    public class CreateImageCommandValidatorTests
    {
        private readonly CreateImageCommandValidator _validator;

        public CreateImageCommandValidatorTests()
        {
            _validator = new CreateImageCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Image_Is_Null()
        {
            var command = new CreateImageCommand(null!);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Image)
                  .WithErrorMessage(string.Format(ErrorMessages.ImageIsRequired));
        }

        [Fact]
        public void Should_Not_Have_Any_Validation_Errors_When_Command_Is_Fully_Valid()
        {
            var validImageDto = new ImageFileBaseCreateDTO
            {
                Alt = "Valid Alt Text",
                Title = "valid_image_title",
                BaseFormat = "jpeg",
                MimeType = "image/jpeg",
                Extension = ".jpg"
            };
            var command = new CreateImageCommand(validImageDto);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}