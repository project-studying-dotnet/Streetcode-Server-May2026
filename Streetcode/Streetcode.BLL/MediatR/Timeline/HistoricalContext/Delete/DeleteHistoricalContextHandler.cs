using MediatR;
using AutoMapper;
using FluentResults;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using HistContext = Streetcode.DAL.Entities.Timeline.HistoricalContext;

namespace Streetcode.BLL.MediatR.Timeline.HistoricalContext.Delete;

public sealed class DeleteHistoricalContextHandler(
    IRepositoryWrapper thisRepositoryWrapper,
    IMapper thisMapper,
    ILoggerService thisLogger)
    : IRequestHandler<DeleteHistoricalContextCommand, Result<HistoricalContextDto>>
{
    public async Task<Result<HistoricalContextDto>> Handle(DeleteHistoricalContextCommand request, CancellationToken cancellationToken)
    {
        HistContext? historical_context = await thisRepositoryWrapper.HistoricalContextRepository.GetFirstOrDefaultAsync(
            predicate: hc => hc.Id == request.Id,
            cancellationToken: cancellationToken);

        if(historical_context is null)
        {
            string error_msg = string.Format(ErrorMessages.HistoricalContextWithIdNotFound, request.Id);
            thisLogger.LogError(request, error_msg);
            return Result.Fail(new Error(error_msg));
        }

        try
        {
            thisRepositoryWrapper.HistoricalContextRepository.Delete(historical_context);
            bool success = await thisRepositoryWrapper.SaveChangesAsync(cancellationToken) > 0;
            if(!success)
            {
                string error_msg = string.Format(ErrorMessages.FailedToDeleteHistoricalContextWithId, request.Id);
                thisLogger.LogError(request, error_msg);
                return Result.Fail(new Error(error_msg));
            }
        }
        catch(Exception ex)
        {
            thisLogger.LogError(request, ex.Message);
            return Result.Fail(new Error(ex.Message));
        }

        HistoricalContextDto deleted_dto = thisMapper.Map<HistoricalContextDto>(historical_context);
        return Result.Ok(deleted_dto);
    }
}