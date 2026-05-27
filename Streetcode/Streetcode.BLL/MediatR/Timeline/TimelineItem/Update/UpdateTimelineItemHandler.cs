using MediatR;
using AutoMapper;
using FluentResults;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Timeline;
using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using HistContext = Streetcode.DAL.Entities.Timeline.HistoricalContext;
using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Update;

public sealed class UpdateTimelineItemHandler(
    IRepositoryWrapper thisRepositoryWrapper,
    IMapper thisMapper,
    ILoggerService thisLogger)
    : IRequestHandler<UpdateTimelineItemCommand, Result<TimelineItemDto>>
{
    public async Task<Result<TimelineItemDto>> Handle(UpdateTimelineItemCommand request, CancellationToken cancellationToken)
    {
        TimelineItemEntity? timeline_item = await thisRepositoryWrapper.TimelineRepository.GetFirstOrDefaultAsync(
            predicate: ti => ti.Id == request.TimelineItem.Id,
            include: ti => ti.Include(til => til.HistoricalContextTimelines),
            cancellationToken: cancellationToken);

        if(timeline_item is null)
        {
            string error_msg = string.Format(ErrorMessages.TimelineItemWithIdNotFound, request.TimelineItem.Id);
            thisLogger.LogError(request, error_msg);
            return Result.Fail(new Error(error_msg));
        }

        thisRepositoryWrapper.HistoricalContextTimelineRepository.DeleteRange(timeline_item.HistoricalContextTimelines);
        await thisRepositoryWrapper.SaveChangesAsync(cancellationToken);

        Result<List<HistoricalContextTimeline>> historical_context_result = await BuildHistoricalContextTimelines(request, timeline_item);
        if(historical_context_result.IsFailed)
        {
            return Result.Fail(historical_context_result.Errors);
        }

        thisMapper.Map(request.TimelineItem, timeline_item);
        timeline_item.HistoricalContextTimelines = historical_context_result.Value;
        try
        {
            thisRepositoryWrapper.TimelineRepository.Update(timeline_item);
            bool success = await thisRepositoryWrapper.SaveChangesAsync(cancellationToken) > 0;
            if (success is false)
            {
                string error_msg = string.Format(ErrorMessages.FailedToUpdateTimelineItemWithId, request.TimelineItem.Id);
                thisLogger.LogError(request, error_msg);
                return Result.Fail(new Error(error_msg));
            }
        }
        catch(Exception ex)
        {
            thisLogger.LogError(request, ex.Message);
            return Result.Fail(new Error(ex.Message));
        }

        TimelineItemDto updated_dto = thisMapper.Map<TimelineItemDto>(timeline_item);
        return Result.Ok(updated_dto);
    }

    private async Task<Result<List<HistoricalContextTimeline>>> BuildHistoricalContextTimelines(
        UpdateTimelineItemCommand request,
        TimelineItemEntity timelineItem)
    {
        List<int> requested_ids = request.TimelineItem.HistoricalContexts.Select(hc => hc.Id).ToList();
        if (requested_ids.Count == 0)
        {
            return Result.Ok(new List<HistoricalContextTimeline>());
        }

        IEnumerable<HistContext> existing_contexts = await thisRepositoryWrapper.HistoricalContextRepository
            .GetAllAsync(hc => requested_ids.Contains(hc.Id));
        List<HistContext> existing_contexts_list = existing_contexts.ToList();

        if (existing_contexts_list.Count != requested_ids.Count)
        {
            string error_msg = ErrorMessages.CannotFindOneOrMoreHistoricalContexts;
            thisLogger.LogError(request, error_msg);
            return Result.Fail(new Error(error_msg));
        }

        List<HistoricalContextTimeline> join_records = existing_contexts_list.Select(hc => new HistoricalContextTimeline
        {
            TimelineId = timelineItem.Id,
            HistoricalContextId = hc.Id,
            Timeline = timelineItem,
            HistoricalContext = hc,
        }).ToList();

        return Result.Ok(join_records);
    }
}
