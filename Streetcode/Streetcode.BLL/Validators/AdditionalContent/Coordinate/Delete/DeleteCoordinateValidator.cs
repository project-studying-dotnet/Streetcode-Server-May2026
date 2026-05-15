using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Delete;

namespace Streetcode.BLL.Validators.AdditionalContent.Coordinate.Delete
{
    /// <summary>
    /// Validator for DeleteCoordinateCommand.
    /// </summary>
    public class DeleteCoordinateValidator : PositiveIdValidator<DeleteCoordinateCommand>
    {
    }
}
