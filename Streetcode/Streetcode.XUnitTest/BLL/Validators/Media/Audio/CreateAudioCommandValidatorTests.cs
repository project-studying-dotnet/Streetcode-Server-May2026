using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.MediatR.Media.Audio.Create;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Media.Audio.Create;
using Xunit;

namespace Streetcode.XUnitTest.Validators.Media.Audio.Create
{
    public class CreateAudioCommandValidatorTests
    {
        private const int MaxDescriptionLength = 2500;
        private readonly CreateAudioCommandValidator _validator;

        public CreateAudioCommandValidatorTests()
        {
            _validator = new CreateAudioCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Audio_Is_Null()
        {
            var command = new CreateAudioCommand(null!);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Audio)
                  .WithErrorMessage(ErrorMessages.AudioIsRequired);
        }

        [Fact]
        public void Should_Have_Error_When_Inner_Audio_Description_Exceeds_Maximum_Length()
        {
            var invalidAudioDto = new AudioFileBaseCreateDTO { Description = new string('A', 2501) };
            var command = new CreateAudioCommand(invalidAudioDto);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Audio.Description)
                  .WithErrorMessage(string.Format(ErrorMessages.DescriptionMustNotExceedCharacters, MaxDescriptionLength));
        }

        [Fact]
        public void Should_Not_Have_Any_Validation_Errors_When_Command_Is_Fully_Valid()
        {
            var validAudioDto = new AudioFileBaseCreateDTO
            {
                Description = "Valid description text",
                Title = "valid_audio_title",
                BaseFormat = "mp3",
                MimeType = "audio/mpeg",
                Extension = ".mp3"
            };
            var command = new CreateAudioCommand(validAudioDto);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}