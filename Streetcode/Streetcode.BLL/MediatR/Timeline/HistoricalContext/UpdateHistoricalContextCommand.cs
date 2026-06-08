using MediatR;
using FluentResults;
using Streetcode.BLL.DTO.Timeline;

namespace Streetcode.BLL.MediatR.Timeline.HistoricalContext;

public sealed record class UpdateHistoricalContextCommand(
    HistoricalContextDto HistoricalContext
) : IRequest<Result<HistoricalContextDto>>;