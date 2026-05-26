using MediatR;
using FluentResults;
using Streetcode.BLL.DTO.Timeline;

namespace Streetcode.BLL.MediatR.Timeline.HistoricalContext.Delete;

public sealed record class DeleteHistoricalContextCommand(int Id)
    : IRequest<Result<HistoricalContextDto>>;