using FluentValidation;
using Streetcode.BLL.MediatR.Media.Audio.Create;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Media.Audio.Create
{
    public class CreateAudioCommandValidator : AbstractValidator<CreateAudioCommand>
    {
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
