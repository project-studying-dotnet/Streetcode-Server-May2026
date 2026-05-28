using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.GetByStreetcodeId;

public record GetTimelineItemsByStreetcodeIdQuery(int StreetcodeId) : IRequest<Result<IEnumerable<TimelineItemDto>>>, IHasStreetcodeId;