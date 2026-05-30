using MediatR;
using FluentResults;
using Streetcode.BLL.DTO.Timeline;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;

public sealed record class CreateTimelineItemCommand(TimelineItemDto TimelineItem)
    : IRequest<Result<TimelineItemDto>>;