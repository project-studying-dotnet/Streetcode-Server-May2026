using FluentValidation;
using Streetcode.BLL.DTO.Media.Audio;

namespace Streetcode.BLL.Validators.Media.Audio.Create
{
    /// <summary>
    /// Validator for CreateCoordinateCommand.
    /// </summary>
    public class AudioFileBaseCreateDtoValidator : AbstractValidator<AudioFileBaseCreateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioFileBaseCreateDtoValidator"/> class.
        /// </summary>
        public AudioFileBaseCreateDtoValidator()
        {
            Include(new FileBaseCreateDtoValidator());

            RuleFor(x => x.Description)
                .MaximumLength(2500)
                .WithMessage("Description must not exceed 1000 characters");
        }
    }
}
