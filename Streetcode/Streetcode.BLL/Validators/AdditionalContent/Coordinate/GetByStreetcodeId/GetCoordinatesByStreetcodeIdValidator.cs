using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.AdditionalContent.Coordinate.GetByStreetcodeId
{
    public class GetCoordinatesByStreetcodeIdValidator
    : PositiveStreetcodeIdValidator<GetCoordinatesByStreetcodeIdQuery>
    {
    }
}
