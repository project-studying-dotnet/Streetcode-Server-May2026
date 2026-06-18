using FluentValidation;
using Streetcode.BLL.DTO.Media.ArtSlidesTemplates;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Media.ArtSlidesTemplates
{
    public class StreetcodeArtSlideTemplateDtoValidator : AbstractValidator<StreetcodeArtSlideTemplateDto>
    {
        private const int MaxNameLength = 255;

        public StreetcodeArtSlideTemplateDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.InvalidId);

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(ErrorMessages.NameIsRequired)
                .MaximumLength(MaxNameLength)
                .WithMessage(string.Format(
                    ErrorMessages.NameMustNotExceedCharacters,
                    MaxNameLength));
        }
    }
}
