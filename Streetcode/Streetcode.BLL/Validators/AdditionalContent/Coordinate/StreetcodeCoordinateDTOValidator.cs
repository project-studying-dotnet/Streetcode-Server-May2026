using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;

namespace Streetcode.BLL.Validators.AdditionalContent.Coordinate
{
    /// <summary>
    /// Validator for StreetcodeCoordinateDTO.
    /// </summary>
    public class StreetcodeCoordinateDTOValidator : AbstractValidator<StreetcodeCoordinateDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreetcodeCoordinateDTOValidator"/> class.
        /// </summary>
        public StreetcodeCoordinateDTOValidator()
        {
            RuleFor(x => x.StreetcodeId).GreaterThan(0);
            Include(new CoordinateDTOValidator());
        }
    }
}
