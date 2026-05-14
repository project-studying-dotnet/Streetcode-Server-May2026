using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;

namespace Streetcode.BLL.Validators.AdditionalContent.Subtitle
{
    public class SubtitleDTOValidator : AbstractValidator<SubtitleDTO>
    {
        public SubtitleDTOValidator()
        {
            RuleFor(x => x.SubtitleText)
                .NotEmpty()
                .WithMessage("Subtitle text is required.")
                .MaximumLength(500)
                .WithMessage("Subtitle text must not exceed 500 characters.");

            RuleFor(x => x.StreetcodeId)
                .GreaterThan(0)
                .WithMessage("StreetcodeId must be greater than 0.");
        }
    }
}
