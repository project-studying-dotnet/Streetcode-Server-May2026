using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.AdditionalContent.Coordinate.GetByStreetcodeId
{
    /// <summary>
    /// Validator for GetCoordinatesByStreetcodeIdQuery.
    /// </summary>
    public class GetCoordinatesByStreetcodeIdValidator
    : PositiveStreetcodeIdValidator<GetCoordinatesByStreetcodeIdQuery>
    {
    }
}
