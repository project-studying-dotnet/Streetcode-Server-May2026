using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Filter;

namespace Streetcode.BLL.Validators.Streetcode.Streetcode.GetByFilter
{
    /// <summary>
    /// Validator for StreetcodeFilterRequestDTO.
    /// </summary>
    public class StreetcodeFilterRequestDtoValidator
        : AbstractValidator<StreetcodeFilterRequestDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreetcodeFilterRequestDtoValidator"/> class.
        /// </summary>
        public StreetcodeFilterRequestDtoValidator()
        {
            RuleFor(x => x.SearchQuery)
                .NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .MaximumLength(255)
                .WithMessage("{PropertyName} must be 255 characters or less.");
        }
    }
}
