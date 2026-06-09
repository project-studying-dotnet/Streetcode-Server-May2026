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

public sealed class UpdateTimelineItemHandler(
    IRepositoryWrapper thisRepositoryWrapper,
    IMapper thisMapper,
    ILoggerService thisLogger
) : IRequestHandler<UpdateTimelineItemCommand, Result<TimelineItemDto>>
{
    #region IRequestHandler<UpdateTimelineItemCommand, Result<TimelineItemDto>>
    public async Task<Result<TimelineItemDto>> Handle(UpdateTimelineItemCommand request, CancellationToken cancellationToken)
    {
        TimelineItemEntity? timeline_item = await thisRepositoryWrapper.TimelineRepository.GetFirstOrDefaultAsync(
            predicate: ti => ti.Id == request.TimelineItem.Id,
            cancellationToken: cancellationToken
        );
        if (timeline_item is null)
        {
            string error_msg = string.Format(ErrorMessages.TypeWithIdNotFound, nameof(TimelineItemEntity), request.TimelineItem.Id);
            thisLogger.LogError(request, error_msg);
            return Result.Fail(new Error(error_msg));
        }

        thisMapper.Map(request.TimelineItem, timeline_item);
        thisRepositoryWrapper.TimelineRepository.Update(timeline_item);
        if (await thisRepositoryWrapper.SaveChangesAsync(cancellationToken) == 0)
        {
            string error_msg = string.Format(ErrorMessages.FailedToUpdateType, nameof(TimelineItemEntity));
            thisLogger.LogError(request, error_msg);
            return Result.Fail(new Error(error_msg));
        }
        List<HistoricalContextTimeline> new_hcts = request.TimelineItem.HistoricalContexts.Select(hc => new HistoricalContextTimeline
        {
            HistoricalContextId = hc.Id,
            TimelineId = timeline_item.Id
        }).ToList();
        List<HistoricalContextTimeline> old_hcts = await thisRepositoryWrapper.HistoricalContextTimelineRepository.FindAll(
            hct => hct.TimelineId == timeline_item.Id
        ).ToListAsync(cancellationToken);

        List<HistoricalContextTimeline> diff_hcts = old_hcts.ExceptBy(
            new_hcts.Select(hct => hct.HistoricalContextId),
            hct => hct.HistoricalContextId
        ).ToList();
        thisRepositoryWrapper.HistoricalContextTimelineRepository.DeleteRange(diff_hcts);
        if (await thisRepositoryWrapper.SaveChangesAsync(cancellationToken) != diff_hcts.Count)
        {
            string error_msg = string.Format(ErrorMessages.FailedToDeleteType, nameof(HistoricalContextTimeline));
            thisLogger.LogError(request, error_msg);
            return Result.Fail(new Error(error_msg));
        }

        diff_hcts = new_hcts.ExceptBy(
            old_hcts.Select(hct => hct.HistoricalContextId),
            hct => hct.HistoricalContextId
        ).ToList();
        await thisRepositoryWrapper.HistoricalContextTimelineRepository.CreateRangeAsync(diff_hcts);
        if (await thisRepositoryWrapper.SaveChangesAsync(cancellationToken) != diff_hcts.Count)
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