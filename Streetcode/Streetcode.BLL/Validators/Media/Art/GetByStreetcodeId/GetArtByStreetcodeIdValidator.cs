using Streetcode.BLL.MediatR.Media.Art.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.Media.Art.GetByStreetcodeId
{
    /// <summary>
    /// Validator for GetArtsByStreetcodeIdQuery.
    /// </summary>
    public class GetArtByStreetcodeIdValidator
    : PositiveStreetcodeIdValidator<GetArtsByStreetcodeIdQuery>
    {
    }
}
