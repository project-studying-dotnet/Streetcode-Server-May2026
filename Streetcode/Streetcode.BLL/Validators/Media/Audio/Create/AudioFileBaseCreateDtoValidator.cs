using FluentValidation;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Media.Audio.Create
{
    /// <summary>
    /// Validator for CreateCoordinateCommand.
    /// </summary>
    public class AudioFileBaseCreateDtoValidator : AbstractValidator<AudioFileBaseCreateDTO>
    {
        private const int MaxDescriptionLength = 2500;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioFileBaseCreateDtoValidator"/> class.
        /// </summary>
        public AudioFileBaseCreateDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            Include(new FileBaseCreateDtoValidator());

            RuleFor(x => x.Description)
                .MaximumLength(MaxDescriptionLength)
                .WithMessage(string.Format(ErrorMessages.DescriptionMustNotExceedCharacters, MaxDescriptionLength));
        }
    }
}
