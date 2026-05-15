using FluentResults;
using MediatR;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Media.Image.Delete;

public record DeleteImageCommand(int Id) : IRequest<Result<Unit>>, IHasId;
