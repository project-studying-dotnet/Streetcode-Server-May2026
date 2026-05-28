using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByTransliterationUrl;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Streetcode.Streetcode.GetByTransliterationUrl
{
    /// <summary>
    /// Validator for <see cref="GetStreetcodeByTransliterationUrlQuery"/>.
    /// </summary>
    public class GetStreetcodeByTransliterationUrlQueryValidator : AbstractValidator<GetStreetcodeByTransliterationUrlQuery>
    {
        private const int MaxUrlLength = 255;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetStreetcodeByTransliterationUrlQueryValidator"/> class.
        /// </summary>
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