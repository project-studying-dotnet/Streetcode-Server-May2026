using Streetcode.BLL.MediatR.Media.Art.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.Media.Art.GetByStreetcodeId
{
    public class GetArtByStreetcodeIdValidator
    : PositiveStreetcodeIdValidator<GetArtsByStreetcodeIdQuery>
    {
    }
}
