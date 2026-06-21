using FluentValidation;
using Streetcode.BLL.DTO.Media.ArtSlides;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Media.ArtSlides.Create
{
    public class CreateStreetcodeArtSlideDtoValidator : AbstractValidator<CreateStreetcodeArtSlideDto>
    {
        public CreateStreetcodeArtSlideDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Index)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ErrorMessages.IndexMustBeGreaterOrEqualToZero);

            RuleFor(x => x.TemplateId)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.InvalidId);

            RuleFor(x => x.StreetcodeId)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.InvalidId);

            RuleFor(x => x.ArtSlideItems)
                .NotNull()
                .WithMessage(ErrorMessages.ArtSlideItemsIsRequired);

            RuleForEach(x => x.ArtSlideItems)
                .SetValidator(new ArtSlideItemDtoValidator());
        }
    }
}