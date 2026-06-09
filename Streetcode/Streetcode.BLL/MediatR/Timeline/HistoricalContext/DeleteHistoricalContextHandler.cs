using MediatR;
using AutoMapper;
using FluentResults;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using HistContext = Streetcode.DAL.Entities.Timeline.HistoricalContext;

namespace Streetcode.BLL.MediatR.Timeline.HistoricalContext;

public sealed class DeleteHistoricalContextHandler(
    IRepositoryWrapper thisRepositoryWrapper,
    IMapper thisMapper,
    ILoggerService thisLogger
) : IRequestHandler<DeleteHistoricalContextCommand, Result<HistoricalContextDto>>
{
    #region IRequestHandler<DeleteHistoricalContextCommand, Result<HistoricalContextDto>>
    public async Task<Result<HistoricalContextDto>> Handle(DeleteHistoricalContextCommand request, CancellationToken cancellationToken)
    {
        HistContext? historical_context = await thisRepositoryWrapper.HistoricalContextRepository.GetFirstOrDefaultAsync(
            predicate: hc => hc.Id == request.Id,
            cancellationToken: cancellationToken
        );
        if (historical_context is null)
        {
            string error_msg = string.Format(ErrorMessages.TypeWithIdNotFound, nameof(HistContext), request.Id);
            thisLogger.LogError(request, error_msg);
            return Result.Fail(new Error(error_msg));
        }
        thisRepositoryWrapper.HistoricalContextRepository.Delete(historical_context);
        if (await thisRepositoryWrapper.SaveChangesAsync(cancellationToken) == 0)
        {
            string error_msg = string.Format(ErrorMessages.FailedToDeleteType, nameof(HistContext));
            thisLogger.LogError(request, error_msg);
            return Result.Fail(new Error(error_msg));
        }
        return Result.Ok(thisMapper.Map<HistoricalContextDto>(historical_context));
    }
    #endregion
}