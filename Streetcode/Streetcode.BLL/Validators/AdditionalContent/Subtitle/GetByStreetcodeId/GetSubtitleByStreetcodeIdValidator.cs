using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.GetByStreetcodeId;
using Streetcode.BLL.MediatR.AdditionalContent.GetById;
using Streetcode.BLL.MediatR.AdditionalContent.Subtitle.GetByStreetcodeId;

namespace Streetcode.BLL.Validators.AdditionalContent.Subtitle.GetById
{
    public class GetSubtitleByStreetcodeIdValidator
        : PositiveStreetcodeIdValidator<GetSubtitlesByStreetcodeIdQuery>
    {
    }
}
