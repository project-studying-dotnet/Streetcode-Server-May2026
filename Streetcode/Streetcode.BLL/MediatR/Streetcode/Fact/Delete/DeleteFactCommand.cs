using MediatR;
using FluentResults;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Delete;

public record DeleteFactCommand(int Id)
    : IRequest<Result<FactDto>>
{
}
