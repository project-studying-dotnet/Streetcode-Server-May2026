using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Delete;
using Streetcode.BLL.MediatR.Media.Audio.Delete;
using Streetcode.BLL.MediatR.Media.Audio.GetBaseAudio;
using Streetcode.BLL.MediatR.Media.Audio.GetById;

namespace Streetcode.BLL.Validators.Media.Audio.GetById
{
    /// <summary>
    /// Validator for GetAudioByIdQuery.
    /// </summary>
    public class GetAudioByIdValidator : PositiveIdValidator<GetAudioByIdQuery>
    {
    }
}
