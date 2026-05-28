using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Create;
using Streetcode.BLL.Resources;

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
                 .WithMessage(ErrorMessages.CoordinateIsRequired)
                 .SetValidator(new StreetcodeCoordinateDtoValidator());
        }
    }
}
