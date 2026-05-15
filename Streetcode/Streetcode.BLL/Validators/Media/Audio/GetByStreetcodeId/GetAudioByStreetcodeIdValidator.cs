using Streetcode.BLL.MediatR.AdditionalContent.Subtitle.GetByStreetcodeId;
using Streetcode.BLL.MediatR.Media.Art.GetByStreetcodeId;
using Streetcode.BLL.MediatR.Media.Audio.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.Media.Audio.GetByStreetcodeId
{
    /// <summary>
    /// Validator for GetAudioByStreetcodeIdQuery.
    /// </summary>
    public class GetAudioByStreetcodeIdValidator
    : PositiveStreetcodeIdValidator<GetAudioByStreetcodeIdQuery>
    {
    }
}
