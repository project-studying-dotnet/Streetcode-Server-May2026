using MediatR;
using AutoMapper;
using FluentResults;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem;

public sealed class DeleteTimelineItemHandler(
    IRepositoryWrapper thisRepositoryWrapper,
    IMapper thisMapper,
    ILoggerService thisLogger
) : IRequestHandler<DeleteTimelineItemCommand, Result<TimelineItemDto>>
{
    #region IRequestHandler<DeleteTimelineItemCommand, Result<TimelineItemDto>>
    public async Task<Result<TimelineItemDto>> Handle(DeleteTimelineItemCommand request, CancellationToken cancellationToken)
    {
        TimelineItemEntity? timeline_item = await thisRepositoryWrapper.TimelineRepository.GetFirstOrDefaultAsync(
            predicate: ti => ti.Id == request.Id,
            cancellationToken: cancellationToken
        );
        if (timeline_item is null)
        {
            string error_msg = string.Format(ErrorMessages.TypeWithIdNotFound, nameof(TimelineItemEntity), request.Id);
            thisLogger.LogError(request, error_msg);
            return Result.Fail(new Error(error_msg));
        }

        thisRepositoryWrapper.TimelineRepository.Delete(timeline_item);
        if (await thisRepositoryWrapper.SaveChangesAsync(cancellationToken) == 0)
        {
            string error_msg = string.Format(ErrorMessages.FailedToDeleteType, nameof(TimelineItemEntity), request.Id);
            thisLogger.LogError(request, error_msg);
            return Result.Fail(new Error(error_msg));
        }
        return Result.Ok(thisMapper.Map<TimelineItemDto>(timeline_item));
    }
    #endregion
}