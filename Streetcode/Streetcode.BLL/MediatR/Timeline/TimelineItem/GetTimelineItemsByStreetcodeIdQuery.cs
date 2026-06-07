using MediatR;
using FluentResults;
using Streetcode.BLL.DTO.Timeline;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem;

public sealed record class GetTimelineItemsByStreetcodeIdQuery(int StreetcodeId) : IRequest<Result<IEnumerable<TimelineItemDto>>>;
