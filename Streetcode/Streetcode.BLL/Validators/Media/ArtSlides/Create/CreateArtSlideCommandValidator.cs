using FluentValidation;
using Streetcode.BLL.MediatR.Media.ArtSlide.Create;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Media.ArtSlides.Create
{
    public class CreateArtSlideCommandValidator : AbstractValidator<CreateArtSlideCommand>
    {
        public CreateArtSlideCommandValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command.Dto)
                .NotNull()
                .WithMessage(ErrorMessages.RequestIsRequired);

            RuleFor(command => command.Dto)
                .SetValidator(new CreateStreetcodeArtSlideDtoValidator());
        }
    }
}