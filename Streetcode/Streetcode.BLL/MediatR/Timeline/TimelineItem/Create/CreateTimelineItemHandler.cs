using MediatR;
using AutoMapper;
using FluentResults;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Update;
using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;

public sealed class CreateTimelineItemHandler(
    IRepositoryWrapper thisRepositoryWrapper,
    IMapper thisMapper,
    IMediator thisMediator,
    ILoggerService thisLogger)
    : IRequestHandler<CreateTimelineItemCommand, Result<TimelineItemDto>>
{
    public async Task<Result<TimelineItemDto>> Handle(CreateTimelineItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            TimelineItemEntity timeline_item = thisMapper.Map<TimelineItemEntity>(request.TimelineItem);
            await thisRepositoryWrapper.TimelineRepository.CreateAsync(timeline_item);
            bool success = await thisRepositoryWrapper.SaveChangesAsync(cancellationToken) > 0;
            if (success is false)
            {
                string error_msg = ErrorMessages.CannotSaveTimelineItem;
                thisLogger.LogError(request, error_msg);
                return Result.Fail(new Error(error_msg));
            }

            TimelineItemDto dto = thisMapper.Map<TimelineItemDto>(timeline_item);
            dto.HistoricalContexts = request.TimelineItem.HistoricalContexts;
            thisRepositoryWrapper.TimelineRepository.Detach(timeline_item);

            UpdateTimelineItemCommand update_command = new(dto);
            return await thisMediator.Send(update_command, cancellationToken);
        }
        catch (Exception ex)
        {
            thisLogger.LogError(request, ex.Message);
            return Result.Fail(new Error(ex.Message));
        }
    }
}