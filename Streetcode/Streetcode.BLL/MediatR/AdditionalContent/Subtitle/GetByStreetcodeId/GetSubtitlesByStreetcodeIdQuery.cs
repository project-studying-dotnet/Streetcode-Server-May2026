using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.AdditionalContent.Subtitles;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.AdditionalContent.Subtitle.GetByStreetcodeId
{
    public record GetSubtitlesByStreetcodeIdQuery(int StreetcodeId) : IRequest<Result<SubtitleDTO>>, IHasStreetcodeId;
}
