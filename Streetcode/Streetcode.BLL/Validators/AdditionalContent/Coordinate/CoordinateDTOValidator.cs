using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.AdditionalContent.Coordinate
{
    /// <summary>
    /// Validator for CoordinateDTO.
    /// </summary>
    public class CoordinateDtoValidator : AbstractValidator<CoordinateDTO>
    {
        private const decimal MinLatitude = -90;
        private const decimal MaxLatitude = 90;
        private const decimal MinLongitude = -180;
        private const decimal MaxLongitude = 180;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoordinateDtoValidator"/> class.
        /// </summary>
        public CoordinateDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(c => c.Latitude)
                .InclusiveBetween(MinLatitude, MaxLatitude)
                .WithMessage(ErrorMessages.LatitudeMustBeBetweenMinus90And90);

            RuleFor(c => c.Longtitude)
                .InclusiveBetween(MinLongitude, MaxLongitude)
                .WithMessage(ErrorMessages.LongitudeMustBeBetweenMinus180And180);
        }
    }
}
