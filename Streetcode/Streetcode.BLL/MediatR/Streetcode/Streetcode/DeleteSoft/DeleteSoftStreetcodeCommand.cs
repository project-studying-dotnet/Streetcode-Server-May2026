using FluentResults;
using MediatR;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Streetcode.Streetcode.DeleteSoft;

public record DeleteSoftStreetcodeCommand(int Id) : IRequest<Result<Unit>>, IHasId;
