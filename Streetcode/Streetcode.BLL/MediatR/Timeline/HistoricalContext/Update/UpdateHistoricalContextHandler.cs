using MediatR;
using AutoMapper;
using FluentResults;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using HistContext = Streetcode.DAL.Entities.Timeline.HistoricalContext;

namespace Streetcode.BLL.MediatR.Timeline.HistoricalContext.Update;

public sealed class UpdateHistoricalContextHandler(
    IRepositoryWrapper thisRepositoryWrapper,
    IMapper thisMapper,
    ILoggerService thisLogger)
    : IRequestHandler<UpdateHistoricalContextCommand, Result<HistoricalContextDto>>
{
    public async Task<Result<HistoricalContextDto>> Handle(UpdateHistoricalContextCommand request, CancellationToken cancellationToken)
    {
        HistContext? historical_context = await thisRepositoryWrapper.HistoricalContextRepository.GetFirstOrDefaultAsync(
            predicate: hc => hc.Id == request.HistoricalContext.Id,
            cancellationToken: cancellationToken);

        if(historical_context is null)
        {
            string error_msg = string.Format(ErrorMessages.HistoricalContextWithIdNotFound, request.HistoricalContext.Id);
            thisLogger.LogError(request, error_msg);
            return Result.Fail(new Error(error_msg));
        }

        thisMapper.Map(request.HistoricalContext, historical_context);
        try
        {
            thisRepositoryWrapper.HistoricalContextRepository.Update(historical_context);
            bool success = await thisRepositoryWrapper.SaveChangesAsync(cancellationToken) > 0;
            if (success is false)
            {
                string error_msg = string.Format(ErrorMessages.FailedToUpdateHistoricalContextWithId, request.HistoricalContext.Id);
                thisLogger.LogError(request, error_msg);
                return Result.Fail(new Error(error_msg));
            }
        }
        catch(Exception ex)
        {
            string error_msg = string.Format(ErrorMessages.FailedToUpdateHistoricalContextWithId, request.HistoricalContext.Id);
            thisLogger.LogError(request, ex.Message);
            return Result.Fail(new Error(ex.Message));
        }

        HistoricalContextDto updated_dto = thisMapper.Map<HistoricalContextDto>(historical_context);
        return Result.Ok(updated_dto);
    }
}