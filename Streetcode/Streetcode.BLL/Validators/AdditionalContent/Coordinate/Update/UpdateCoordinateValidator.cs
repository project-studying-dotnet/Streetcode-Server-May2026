using FluentValidation;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Update;

namespace Streetcode.BLL.Validators.AdditionalContent.Coordinate.Update
{
    public class UpdateCoordinateValidator : AbstractValidator<UpdateCoordinateCommand>
    {
        public UpdateCoordinateValidator()
        {
            RuleFor(x => x.StreetcodeCoordinate).NotNull();

            RuleFor(x => x.StreetcodeCoordinate.Id)
                .GreaterThan(0)
                .WithMessage("Coordinate Id must be greater than 0 for update operations.");

            RuleFor(x => x.StreetcodeCoordinate)
                .SetValidator(new StreetcodeCoordinateDTOValidator());
        }
    }
}
