using MediatR;
using AutoMapper;
using FluentResults;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Timeline;
using Microsoft.EntityFrameworkCore;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using HistContext = Streetcode.DAL.Entities.Timeline.HistoricalContext;

namespace Streetcode.BLL.MediatR.Timeline.HistoricalContext;

public sealed class GetAllHistoricalContextHandler(
    IRepositoryWrapper thisRepositoryWrapper,
    IMapper thisMapper,
    ILoggerService thisLogger
) : IRequestHandler<GetAllHistoricalContextQuery, Result<IEnumerable<HistoricalContextDto>>>
{
    #region IRequestHandler<GetAllHistoricalContextQuery, Result<IEnumerable<HistoricalContextDto>>>
    public async Task<Result<IEnumerable<HistoricalContextDto>>> Handle(GetAllHistoricalContextQuery request, CancellationToken cancellationToken)
    {
        List<HistContext> historical_contexts = await thisRepositoryWrapper.HistoricalContextRepository
            .FindAll()
            .ToListAsync(cancellationToken);
        if (historical_contexts.Count == 0)
        {
            string error_msg = string.Format(ErrorMessages.FailedToFindAnyType, nameof(HistContext));
            thisLogger.LogError(request, error_msg);
            return Result.Fail(new Error(error_msg));
        }
        return Result.Ok(thisMapper.Map<IEnumerable<HistoricalContextDto>>(historical_contexts));
    }
    #endregion
}