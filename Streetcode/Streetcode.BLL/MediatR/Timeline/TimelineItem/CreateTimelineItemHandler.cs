using MediatR;
using AutoMapper;
using FluentResults;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Timeline;
using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem;

public sealed class CreateTimelineItemHandler(
    IRepositoryWrapper thisRepositoryWrapper,
    IMapper thisMapper,
    ILoggerService thisLogger
) : IRequestHandler<CreateTimelineItemCommand, Result<TimelineItemDto>>
{
    #region IRequestHandler<CreateTimelineItemCommand, Result<TimelineItemDto>>
    public async Task<Result<TimelineItemDto>> Handle(CreateTimelineItemCommand request, CancellationToken cancellationToken)
    {
        TimelineItemEntity timeline_item = thisMapper.Map<TimelineItemEntity>(request.TimelineItem);
        await thisRepositoryWrapper.TimelineRepository.CreateAsync(timeline_item);
        if (await thisRepositoryWrapper.SaveChangesAsync(cancellationToken) == 0)
        {
            string error_msg = string.Format(ErrorMessages.FailedToCreateType, nameof(TimelineItemEntity));
            thisLogger.LogError(request, error_msg);
            return Result.Fail(new Error(error_msg));
        }

        List<HistoricalContextTimeline> hcts = request.TimelineItem.HistoricalContexts.Select(hc => new HistoricalContextTimeline
        {
            HistoricalContextId = hc.Id,
            TimelineId = timeline_item.Id
        }).ToList();
        await thisRepositoryWrapper.HistoricalContextTimelineRepository.CreateRangeAsync(hcts);
        if (await thisRepositoryWrapper.SaveChangesAsync(cancellationToken) != hcts.Count)
        {
            string error_msg = string.Format(ErrorMessages.FailedToCreateType, nameof(HistoricalContextTimeline));
            thisLogger.LogError(request, error_msg);
            return Result.Fail(new Error(error_msg));
        }

        timeline_item.HistoricalContextTimelines = await thisRepositoryWrapper.HistoricalContextTimelineRepository.FindAll(
            hct => hct.TimelineId == timeline_item.Id
        ).Include(hct => hct.HistoricalContext).ToListAsync(cancellationToken);
        return Result.Ok(thisMapper.Map<TimelineItemDto>(timeline_item));
    }
    #endregion
}