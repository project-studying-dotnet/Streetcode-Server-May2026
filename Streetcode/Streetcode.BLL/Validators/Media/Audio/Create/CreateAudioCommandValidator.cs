using FluentValidation;
using Streetcode.BLL.MediatR.Media.Audio.Create;
using Streetcode.BLL.Resources;

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
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Audio)
                .NotNull()
                .WithMessage(ErrorMessages.AudioIsRequired)
                .SetValidator(new AudioFileBaseCreateDtoValidator());
        }
    }
}
