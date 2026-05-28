using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Create;

namespace Streetcode.BLL.Validators.AdditionalContent.Coordinate.Create
{
    /// <summary>
    /// Validator for CreateCoordinateCommand.
    /// </summary>
    public class CreateCoordinateValidator : AbstractValidator<CreateCoordinateCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCoordinateValidator"/> class.
        /// </summary>
        public CreateCoordinateValidator()
        {
            RuleFor(x => x.StreetcodeCoordinate)
            .NotNull()
            .SetValidator(new StreetcodeCoordinateDtoValidator());
        }
    }
}
