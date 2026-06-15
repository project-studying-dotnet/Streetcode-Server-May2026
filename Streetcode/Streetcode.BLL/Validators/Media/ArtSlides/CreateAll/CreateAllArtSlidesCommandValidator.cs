using FluentValidation;
using Streetcode.BLL.MediatR.Media.ArtSlide.CreateAll;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Validators.Media.ArtSlides.Create;

namespace Streetcode.BLL.Validators.Media.ArtSlides
{
    public class CreateAllArtSlidesCommandValidator : AbstractValidator<CreateAllArtSlidesCommand>
    {
        public CreateAllArtSlidesCommandValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(command => command.ArtSlides)
                .NotNull()
                .WithMessage(ErrorMessages.ArtSlideItemsIsRequired)
                .NotEmpty()
                .WithMessage(ErrorMessages.ArtSlideItemsIsRequired);

            RuleForEach(command => command.ArtSlides)
                .SetValidator(new CreateStreetcodeArtSlideDtoValidator());
        }
    }
}