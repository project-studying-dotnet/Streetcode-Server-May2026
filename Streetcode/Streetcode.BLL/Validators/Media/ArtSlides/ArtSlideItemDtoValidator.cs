using FluentValidation;
using Streetcode.BLL.DTO.Media.ArtSlides;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Media.ArtSlides
{
    public class ArtSlideItemDtoValidator : AbstractValidator<ArtSlideItemDto>
    {
        public ArtSlideItemDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.ArtId)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.InvalidId);

            RuleFor(x => x.Index)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ErrorMessages.IndexMustBeGreaterOrEqualToZero);
        }
    }
}