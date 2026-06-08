using MediatR;
using FluentResults;
using Streetcode.BLL.DTO.Timeline;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem;

public sealed record class DeleteTimelineItemCommand(int Id) : IRequest<Result<TimelineItemDto>>;