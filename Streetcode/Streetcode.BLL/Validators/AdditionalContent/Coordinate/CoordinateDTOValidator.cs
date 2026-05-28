using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates;

namespace Streetcode.BLL.Validators.AdditionalContent.Coordinate
{
    /// <summary>
    /// Validator for CoordinateDTO.
    /// </summary>
    public class CoordinateDtoValidator : AbstractValidator<CoordinateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoordinateDtoValidator"/> class.
        /// </summary>
        public CoordinateDtoValidator()
        {
            RuleFor(c => c.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude must be between -90 and 90.");

            RuleFor(c => c.Longtitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude must be between -180 and 180.");
        }
    }
}
