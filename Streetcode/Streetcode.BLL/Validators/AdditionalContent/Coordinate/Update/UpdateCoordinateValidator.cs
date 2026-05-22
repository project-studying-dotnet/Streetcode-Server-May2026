using FluentValidation;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Update;

namespace Streetcode.BLL.Validators.AdditionalContent.Coordinate.Update
{
    /// <summary>
    /// Validator for UpdateCoordinateCommand.
    /// </summary>
    public class UpdateCoordinateValidator : AbstractValidator<UpdateCoordinateCommand>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCoordinateValidator"/> class.
        /// </summary>
        public UpdateCoordinateValidator()
        {
            RuleFor(x => x.StreetcodeCoordinate).NotNull();

            RuleFor(x => x.StreetcodeCoordinate.Id)
                .GreaterThan(0)
                .When(x => x.StreetcodeCoordinate != null)
                .WithMessage("Coordinate Id must be greater than 0 for update operations.");

            RuleFor(x => x.StreetcodeCoordinate)
                .SetValidator(new StreetcodeCoordinateDTOValidator());
        }
    }
}
