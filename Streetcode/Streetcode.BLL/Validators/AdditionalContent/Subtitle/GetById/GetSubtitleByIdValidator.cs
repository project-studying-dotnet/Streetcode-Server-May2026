using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.GetByStreetcodeId;
using Streetcode.BLL.MediatR.AdditionalContent.GetById;

namespace Streetcode.BLL.Validators.AdditionalContent.Subtitle.GetById
{
    public class GetSubtitleByIdValidator
        : PositiveIdValidator<GetSubtitleByIdQuery>
    {
    }
}
