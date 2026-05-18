using FluentValidation;
using Streetcode.BLL.DTO.Streetcode;

namespace Streetcode.BLL.Validators.Partners
{
    /// <summary>
    /// Validator for StreetcodeShortDTO.
    /// </summary>
    public class StreetcodeShortDtoValidator : AbstractValidator<StreetcodeShortDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreetcodeShortDtoValidator"/> class.
        /// </summary>
        public StreetcodeShortDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(255);
        }
    }
}