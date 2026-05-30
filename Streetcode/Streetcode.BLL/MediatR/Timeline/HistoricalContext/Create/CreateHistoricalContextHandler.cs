using MediatR;
using AutoMapper;
using FluentResults;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using HistContext = Streetcode.DAL.Entities.Timeline.HistoricalContext;

namespace Streetcode.BLL.MediatR.Timeline.HistoricalContext.Create;

public sealed class CreateHistoricalContextHandler(
    IRepositoryWrapper thisRepositoryWrapper,
    IMapper thisMapper,
    ILoggerService thisLogger)
    : IRequestHandler<CreateHistoricalContextCommand, Result<HistoricalContextDto>>
{
    public async Task<Result<HistoricalContextDto>> Handle(CreateHistoricalContextCommand request, CancellationToken cancellationToken)
    {
        try
        {
            HistContext historical_context = thisMapper.Map<HistContext>(request.HistoricalContext);
            await thisRepositoryWrapper.HistoricalContextRepository.CreateAsync(historical_context);
            bool success = await thisRepositoryWrapper.SaveChangesAsync(cancellationToken) > 0;
            if(!success)
            {
                string error_msg = ErrorMessages.CannotSaveHistoricalContextToDatabase;
                thisLogger.LogError(request, error_msg);
                return Result.Fail(new Error(error_msg));
            }

            return Result.Ok(thisMapper.Map<HistoricalContextDto>(historical_context));
        }
        catch (Exception ex)
        {
            thisLogger.LogError(request, ex.Message);
            return Result.Fail(new Error(ex.Message));
        }
    }
}