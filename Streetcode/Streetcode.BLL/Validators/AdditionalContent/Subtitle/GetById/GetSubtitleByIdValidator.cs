using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.GetByStreetcodeId;
using Streetcode.BLL.MediatR.AdditionalContent.GetById;

namespace Streetcode.BLL.Validators.AdditionalContent.Subtitle.GetById
{
    /// <summary>
    /// Validator for GetSubtitleByIdQuery.
    /// </summary>
    public class GetSubtitleByIdValidator
        : PositiveIdValidator<GetSubtitleByIdQuery>
    {
    }
}
