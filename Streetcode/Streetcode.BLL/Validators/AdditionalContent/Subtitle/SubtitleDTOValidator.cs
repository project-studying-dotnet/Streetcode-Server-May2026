using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.AdditionalContent.Subtitle
{
    public class SubtitleDtoValidator : AbstractValidator<SubtitleDTO>
    {
        public SubtitleDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.SubtitleText)
                .NotEmpty()
                .WithMessage(ErrorMessages.SubtitleTextIsRequired);

            RuleFor(x => x.StreetcodeId)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.StreetcodeIdMustBePositive);
        }
    }
}
