using FluentResults;
using MediatR;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Delete;

public record DeleteCoordinateCommand(int Id) : IRequest<Result<Unit>>, IHasId;