using FluentValidation;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Create;
using Streetcode.BLL.Resources;

namespace Streetcode.BLL.Validators.AdditionalContent.Coordinate.Create
{
    public class CreateCoordinateValidator : AbstractValidator<CreateCoordinateCommand>
    {
        public CreateCoordinateValidator()
        {
            RuleFor(x => x.StreetcodeCoordinate)
                 .NotNull()
                 .WithMessage(ErrorMessages.CoordinateIsRequired)
                 .SetValidator(new StreetcodeCoordinateDtoValidator());
        }
    }
}
