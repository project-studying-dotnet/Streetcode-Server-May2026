using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Media.Audio.Create;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Media.Audio.Create
{
    public class AudioFileBaseCreateDtoValidatorTests
    {
        private const int MaxDescriptionLength = 2500;
        private readonly AudioFileBaseCreateDtoValidator _validator;

        public AudioFileBaseCreateDtoValidatorTests()
        {
            _validator = new AudioFileBaseCreateDtoValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Description_Exceeds_Maximum_Length()
        {
            var longDescription = new string('A', 2501);
            var dto = new AudioFileBaseCreateDTO { Description = longDescription };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Description)
                  .WithErrorMessage(string.Format(ErrorMessages.DescriptionMustNotExceedCharacters, MaxDescriptionLength));
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Within_Allowed_Length()
        {
            var validDescription = new string('A', 2500);
            var dto = new AudioFileBaseCreateDTO { Description = validDescription };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }
    }
}
