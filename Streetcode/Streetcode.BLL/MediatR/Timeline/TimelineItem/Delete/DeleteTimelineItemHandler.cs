using MediatR;
using AutoMapper;
using FluentResults;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Delete;

public sealed class DeleteTimelineItemHandler(
    IRepositoryWrapper thisRepositoryWrapper,
    IMapper thisMapper,
    ILoggerService thisLogger)
    : IRequestHandler<DeleteTimelineItemCommand, Result<TimelineItemDto>>
{
    public async Task<Result<TimelineItemDto>> Handle(DeleteTimelineItemCommand request, CancellationToken cancellationToken)
    {
        TimelineItemEntity? timeline_item = await thisRepositoryWrapper.TimelineRepository.GetFirstOrDefaultAsync(
            predicate: ti => ti.Id == request.Id,
            cancellationToken: cancellationToken);

        if (timeline_item is null)
        {
            string error_msg = string.Format(ErrorMessages.TimelineItemWithIdNotFound, request.Id);
            thisLogger.LogError(request, error_msg);
            return Result.Fail(new Error(error_msg));
        }

        try
        {
            thisRepositoryWrapper.TimelineRepository.Delete(timeline_item);
            bool success = await thisRepositoryWrapper.SaveChangesAsync(cancellationToken) > 0;
            if(success is false)
            {
                string error_msg = string.Format(ErrorMessages.FailedToDeleteTimelineItemWithId, request.Id);
                thisLogger.LogError(request, error_msg);
                return Result.Fail(new Error(error_msg));
            }
        }
        catch (Exception ex)
        {
            thisLogger.LogError(request, ex.Message);
            return Result.Fail(new Error(ex.Message));
        }

        TimelineItemDto deleted_dto = thisMapper.Map<TimelineItemDto>(timeline_item);
        return Result.Ok(deleted_dto);
    }
}