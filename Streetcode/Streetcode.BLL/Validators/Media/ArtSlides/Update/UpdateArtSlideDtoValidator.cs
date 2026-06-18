using FluentValidation;
using Streetcode.BLL.DTO.Media.ArtSlides;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Media.ArtSlides.Update
{
    public class UpdateArtSlideDtoValidator : AbstractValidator<UpdateArtSlideDto>
    {
        public UpdateArtSlideDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.InvalidId);

            RuleFor(x => x.Index)
                .GreaterThanOrEqualTo(0)
                .WithMessage(ErrorMessages.IndexMustBeGreaterOrEqualToZero);

            RuleFor(x => x.TemplateId)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.InvalidId);

            RuleFor(x => x.StreetcodeId)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.InvalidId);

            RuleForEach(x => x.ArtSlideItems)
                .SetValidator(new ArtSlideItemDtoValidator());
        }
    }
}