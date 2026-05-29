using FluentValidation;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Partners
{
    public class StreetcodeShortDtoValidator : AbstractValidator<StreetcodeShortDTO>
    {
        private const int MaxTitleLength = 255;
        public StreetcodeShortDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.IdMustBePositive);

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage(ErrorMessages.TitleIsRequired)
                .MaximumLength(MaxTitleLength)
                .WithMessage(string.Format(ErrorMessages.TitleMustNotExceedCharacters, MaxTitleLength));
        }
    }
}