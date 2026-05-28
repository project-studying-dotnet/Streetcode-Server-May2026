using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Filter;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.Streetcode.Streetcode.GetByFilter
{
    /// <summary>
    /// Validator for StreetcodeFilterRequestDTO.
    /// </summary>
    public class StreetcodeFilterRequestDtoValidator
        : AbstractValidator<StreetcodeFilterRequestDTO>
    {
        private const int MaxSearchQueryLength = 255;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreetcodeFilterRequestDtoValidator"/> class.
        /// </summary>
        public StreetcodeFilterRequestDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.SearchQuery)
                .NotEmpty()
                .WithMessage(ErrorMessages.SearchQueryIsRequired)
                .MaximumLength(MaxSearchQueryLength)
                .WithMessage(string.Format(ErrorMessages.SearchQueryMustNotExceedCharacters, MaxSearchQueryLength));
        }
    }
}