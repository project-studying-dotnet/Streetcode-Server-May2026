using MediatR;
using AutoMapper;
using FluentResults;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using HistContext = Streetcode.DAL.Entities.Timeline.HistoricalContext;

namespace Streetcode.BLL.MediatR.Timeline.HistoricalContext;

public sealed class UpdateHistoricalContextHandler(
    IRepositoryWrapper thisRepositoryWrapper,
    IMapper thisMapper,
    ILoggerService thisLogger
) : IRequestHandler<UpdateHistoricalContextCommand, Result<HistoricalContextDto>>
{
    #region IRequestHandler<UpdateHistoricalContextCommand, Result<HistoricalContextDto>>
    public async Task<Result<HistoricalContextDto>> Handle(UpdateHistoricalContextCommand request, CancellationToken cancellationToken)
    {
        HistContext? historical_context = await thisRepositoryWrapper.HistoricalContextRepository.GetFirstOrDefaultAsync(
            predicate: hc => hc.Id == request.HistoricalContext.Id,
            cancellationToken: cancellationToken
        );
        if (historical_context is null)
        {
            string error_msg = string.Format(ErrorMessages.TypeWithIdNotFound, nameof(HistContext), request.HistoricalContext.Id);
            thisLogger.LogError(request, error_msg);
            return Result.Fail(new Error(error_msg));
        }
        thisMapper.Map(request.HistoricalContext, historical_context);
        thisRepositoryWrapper.HistoricalContextRepository.Update(historical_context);
        if (await thisRepositoryWrapper.SaveChangesAsync(cancellationToken) == 0)
        {
            string error_msg = string.Format(ErrorMessages.FailedToUpdateType, nameof(HistContext));
            thisLogger.LogError(request, error_msg);
            return Result.Fail(new Error(error_msg));
        }
        return Result.Ok(thisMapper.Map<HistoricalContextDto>(historical_context));
    }
    #endregion
}