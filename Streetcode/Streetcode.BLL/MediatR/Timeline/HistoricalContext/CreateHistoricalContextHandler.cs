using MediatR;
using AutoMapper;
using FluentResults;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using HistContext = Streetcode.DAL.Entities.Timeline.HistoricalContext;

namespace Streetcode.BLL.MediatR.Timeline.HistoricalContext;

public sealed class CreateHistoricalContextHandler(
    IRepositoryWrapper thisRepositoryWrapper,
    IMapper thisMapper,
    ILoggerService thisLogger
) : IRequestHandler<CreateHistoricalContextCommand, Result<HistoricalContextDto>>
{
    #region IRequestHandler<CreateHistoricalContextCommand, Result<HistoricalContextDto>>
    public async Task<Result<HistoricalContextDto>> Handle(CreateHistoricalContextCommand request, CancellationToken cancellationToken)
    {
        HistContext historical_context = thisMapper.Map<HistContext>(request.HistoricalContext);
        await thisRepositoryWrapper.HistoricalContextRepository.CreateAsync(historical_context);
        if (await thisRepositoryWrapper.SaveChangesAsync(cancellationToken) == 0)
        {
            string error_msg = string.Format(ErrorMessages.FailedToCreateType, nameof(HistContext));
            thisLogger.LogError(request, error_msg);
            return Result.Fail(new Error(error_msg));
        }
        return Result.Ok(thisMapper.Map<HistoricalContextDto>(historical_context));
    }
    #endregion
}