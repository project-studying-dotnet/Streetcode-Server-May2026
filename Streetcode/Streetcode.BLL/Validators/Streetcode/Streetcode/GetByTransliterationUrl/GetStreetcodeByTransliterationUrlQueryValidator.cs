using FluentValidation;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByTransliterationUrl;

namespace Streetcode.BLL.Validators.Streetcode.Streetcode.GetByTransliterationUrl
{
    /// <summary>
    /// Validator for <see cref="GetStreetcodeByTransliterationUrlQuery"/>.
    /// </summary>
    public class GetStreetcodeByTransliterationUrlQueryValidator : AbstractValidator<GetStreetcodeByTransliterationUrlQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetStreetcodeByTransliterationUrlQueryValidator"/> class.
        /// </summary>
        public GetStreetcodeByTransliterationUrlQueryValidator()
        {
            RuleFor(x => x.url)
                .NotEmpty()
                .WithMessage("Transliteration url is required")
                .MaximumLength(255)
                .WithMessage("Transliteration url must not exceed 255 characters");
        }
    }
}
