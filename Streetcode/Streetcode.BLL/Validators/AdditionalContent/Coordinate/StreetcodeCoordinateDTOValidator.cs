using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.AdditionalContent.Coordinate
{
    public class StreetcodeCoordinateDtoValidator : AbstractValidator<StreetcodeCoordinateDTO>
    {
        public StreetcodeCoordinateDtoValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.StreetcodeId)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.StreetcodeIdMustBePositive);
            Include(new CoordinateDtoValidator());
        }
    }
}
