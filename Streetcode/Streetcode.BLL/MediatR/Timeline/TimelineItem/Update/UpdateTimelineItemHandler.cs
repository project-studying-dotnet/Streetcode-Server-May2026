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

        Result<List<HistoricalContextTimeline>> historical_context_result = await RetrieveHistoricalContextTimelines(request, timeline_item, cancellationToken);
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

    private async Task<Result<List<HistoricalContextTimeline>>> RetrieveHistoricalContextTimelines(
        UpdateTimelineItemCommand request,
        TimelineItemEntity timelineItem,
        CancellationToken cancellationToken)
    {
        List<Task<Result<HistoricalContextTimeline>>> historical_context_timelines = request.TimelineItem.HistoricalContexts.Select(async r =>
        {
            HistContext? historical_context = await thisRepositoryWrapper.HistoricalContextRepository.GetFirstOrDefaultAsync(
                predicate: hc => hc.Id == r.Id,
                cancellationToken: cancellationToken);

            if (historical_context is null)
            {
                string error_msg = string.Format(ErrorMessages.HistoricalContextWithIdNotFound, r.Id);
                thisLogger.LogError(request, error_msg);
                return Result.Fail(new Error(error_msg));
            }

            HistoricalContextTimeline timeline = new()
            {
                HistoricalContext = historical_context,
                Timeline = timelineItem,
            };
            return Result.Ok(timeline);
        }).ToList();
        Result<HistoricalContextTimeline>[] historical_context_timeline_results = await Task.WhenAll(historical_context_timelines);
        if (historical_context_timeline_results.Any(r => r.IsFailed) is true)
        {
            string error_msg = ErrorMessages.CannotFindOneOrMoreHistoricalContexts;
            thisLogger.LogError(request, error_msg);
            return Result.Fail(new Error(error_msg));
        }

        return Result.Ok(historical_context_timeline_results.Select(r => r.Value).ToList());
    }
}