using FluentValidation;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Update;
using Streetcode.BLL.Resources;

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
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.StreetcodeCoordinate)
                .NotNull()
                .WithMessage(ErrorMessages.CoordinateIsRequired);

            RuleFor(x => x.StreetcodeCoordinate.Id)
                .GreaterThan(0)
                .WithMessage(ErrorMessages.CoordinateIdMustBePositive);

            RuleFor(x => x.StreetcodeCoordinate)
                .SetValidator(new StreetcodeCoordinateDtoValidator());
        }
    }
}
