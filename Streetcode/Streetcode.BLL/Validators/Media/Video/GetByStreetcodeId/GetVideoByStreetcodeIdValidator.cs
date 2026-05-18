using Streetcode.BLL.MediatR.Media.Video.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.Media.Video.GetByStreetcodeId
{
    /// <summary>
    /// Validator for GetVideoByStreetcodeIdQuery.
    /// </summary>
    public class GetVideoByStreetcodeIdValidator : PositiveStreetcodeIdValidator<GetVideoByStreetcodeIdQuery>
    {
    }
}
