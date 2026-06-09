using Streetcode.BLL.MediatR.Media.Art.GetById;
using Streetcode.BLL.MediatR.Media.StreetcodeArt.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.Media.StreetcodeArt.GetByStreetcodeId
{
    public class GetStreetcodeArtByStreetcodeIdValidator
     : PositiveStreetcodeIdValidator<GetStreetcodeArtByStreetcodeIdQuery>
    {
    }
}
