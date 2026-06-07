using MediatR;
using FluentResults;
using Streetcode.BLL.DTO.Timeline;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem;

public sealed record class UpdateTimelineItemCommand(TimelineItemDto TimelineItem) : IRequest<Result<TimelineItemDto>>;