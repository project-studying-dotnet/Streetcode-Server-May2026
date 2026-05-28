using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.Resources;

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
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.StreetcodeId)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.StreetcodeIdMustBePositive);
            Include(new CoordinateDtoValidator());
        }
    }
}
