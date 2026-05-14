using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;

namespace Streetcode.BLL.Validators.AdditionalContent.Coordinate
{
    public class StreetcodeCoordinateDTOValidator : AbstractValidator<StreetcodeCoordinateDTO>
    {
        public StreetcodeCoordinateDTOValidator()
        {
            RuleFor(x => x.StreetcodeId).GreaterThan(0);
            Include(new CoordinateDTOValidator());
        }
    }
}
