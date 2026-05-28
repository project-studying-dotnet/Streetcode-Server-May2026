using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.AdditionalContent.Subtitle
{
    /// <summary>
    /// Validator for SubtitleDTO.
    /// </summary>
    public class SubtitleDtoValidator : AbstractValidator<SubtitleDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubtitleDtoValidator"/> class.
        /// </summary>
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
