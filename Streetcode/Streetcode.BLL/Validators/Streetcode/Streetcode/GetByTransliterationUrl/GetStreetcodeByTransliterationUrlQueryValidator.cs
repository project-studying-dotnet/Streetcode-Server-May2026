using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByTransliterationUrl;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Streetcode.Streetcode.GetByTransliterationUrl
{
    public class GetStreetcodeByTransliterationUrlQueryValidator : AbstractValidator<GetStreetcodeByTransliterationUrlQuery>
    {
        private const int MaxUrlLength = 255;
        public GetStreetcodeByTransliterationUrlQueryValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.url)
                .NotEmpty()
                .WithMessage(ErrorMessages.TransliterationUrlIsRequired)
                .MaximumLength(MaxUrlLength)
                .WithMessage(string.Format(ErrorMessages.TransliterationUrlMustNotExceedCharacters, MaxUrlLength));
        }
    }
}