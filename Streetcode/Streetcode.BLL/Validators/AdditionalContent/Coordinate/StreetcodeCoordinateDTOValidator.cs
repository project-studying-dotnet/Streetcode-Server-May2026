using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;

namespace Streetcode.BLL.Validators.AdditionalContent.Coordinate
{
    /// <summary>
    /// Validator for StreetcodeCoordinateDTO.
    /// </summary>
    public class StreetcodeCoordinateDtoValidator : AbstractValidator<StreetcodeCoordinateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreetcodeCoordinateDtoValidator"/> class.
        /// </summary>
        public StreetcodeCoordinateDtoValidator()
        {
            RuleFor(x => x.StreetcodeId).GreaterThan(0);
            Include(new CoordinateDtoValidator());
        }
    }
}
