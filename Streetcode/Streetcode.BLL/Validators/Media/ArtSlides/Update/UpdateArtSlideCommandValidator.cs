using FluentValidation;
using Streetcode.BLL.MediatR.Media.ArtSlide.Update;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Media.ArtSlides.Update;

namespace Streetcode.BLL.Validators.Media.ArtSlides
{
    public class UpdateArtSlideCommandValidator : AbstractValidator<UpdateArtSlideCommand>
    {
        public UpdateArtSlideCommandValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command.Dto)
                .NotNull()
                .WithMessage(ErrorMessages.RequestIsRequired);

            RuleFor(command => command.Dto)
                .SetValidator(new UpdateArtSlideDtoValidator());
        }
    }
}