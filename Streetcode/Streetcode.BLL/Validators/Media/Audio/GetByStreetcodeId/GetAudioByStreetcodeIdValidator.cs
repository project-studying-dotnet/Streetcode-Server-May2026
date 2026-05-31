using Streetcode.BLL.MediatR.Media.Audio.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.Media.Audio.GetByStreetcodeId
{
    public class GetAudioByStreetcodeIdValidator
    : PositiveStreetcodeIdValidator<GetAudioByStreetcodeIdQuery>
    {
    }
}
