using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;

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
            RuleFor(x => x.SubtitleText)
                .NotEmpty()
                .WithMessage("Subtitle text is required.");

            RuleFor(x => x.StreetcodeId)
                .GreaterThan(0)
                .WithMessage("StreetcodeId must be greater than 0.");
        }
    }
}
