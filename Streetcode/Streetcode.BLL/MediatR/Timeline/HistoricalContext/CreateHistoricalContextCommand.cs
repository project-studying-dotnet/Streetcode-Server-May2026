using MediatR;
using FluentResults;
using Streetcode.BLL.DTO.Timeline;

namespace Streetcode.BLL.MediatR.Timeline.HistoricalContext;

public sealed record class CreateHistoricalContextCommand(
    HistoricalContextDto HistoricalContext
) : IRequest<Result<HistoricalContextDto>>;