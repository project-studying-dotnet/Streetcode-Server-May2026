using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Delete;
using Streetcode.BLL.MediatR.Media.Audio.Delete;
using Streetcode.BLL.MediatR.Media.Audio.GetBaseAudio;

namespace Streetcode.BLL.Validators.Media.Audio.GetBase
{
    /// <summary>
    /// Validator for GetBaseAudioQuery.
    /// </summary>
    public class GetAudioByIdValidator : PositiveIdValidator<GetBaseAudioQuery>
    {
    }
}
