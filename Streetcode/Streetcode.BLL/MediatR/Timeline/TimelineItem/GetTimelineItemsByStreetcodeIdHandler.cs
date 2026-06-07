using MediatR;
using AutoMapper;
using FluentResults;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Timeline;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem;

public sealed class GetTimelineItemsByStreetcodeIdHandler(
    IRepositoryWrapper thisRepositoryWrapper,
    IMapper thisMapper,
    ILoggerService thisLogger
) : IRequestHandler<GetTimelineItemsByStreetcodeIdQuery, Result<IEnumerable<TimelineItemDto>>>
{
    #region IRequestHandler<GetTimelineItemsByStreetcodeIdQuery, Result<IEnumerable<TimelineItemDto>>>
    public async Task<Result<IEnumerable<TimelineItemDto>>> Handle(GetTimelineItemsByStreetcodeIdQuery request, CancellationToken cancellationToken)
    {
        List<TimelineItemEntity> timeline_items = await thisRepositoryWrapper.TimelineRepository
            .FindAll(ti => ti.StreetcodeId == request.StreetcodeId)
            .Include(ti => ti.HistoricalContextTimelines)
            .ThenInclude(hct => hct.HistoricalContext)
            .ToListAsync(cancellationToken);
        if (timeline_items.Count == 0)
        {
            string error_msg = string.Format(ErrorMessages.FailedToFindAnyType, nameof(TimelineItemEntity));
            thisLogger.LogError(request, error_msg);
            return Result.Fail(new Error(error_msg));
        }
        return Result.Ok(thisMapper.Map<IEnumerable<TimelineItemDto>>(timeline_items));
    }
    #endregion
}