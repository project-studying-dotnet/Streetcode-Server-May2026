using FluentValidation;
using Streetcode.BLL.MediatR.Media.Audio.Create;

namespace Streetcode.BLL.Validators.Media.Audio.Create
{
    /// <summary>
    /// Validator for CreateAudioCommand.
    /// </summary>
    public class CreateAudioCommandValidator : AbstractValidator<CreateAudioCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateAudioCommandValidator"/> class.
        /// </summary>
        public CreateAudioCommandValidator()
        {
            RuleFor(x => x.Audio)
                .NotNull()
                .WithMessage("Audio is required")
                .SetValidator(new AudioFileBaseCreateDtoValidator());
        }
    }
}
