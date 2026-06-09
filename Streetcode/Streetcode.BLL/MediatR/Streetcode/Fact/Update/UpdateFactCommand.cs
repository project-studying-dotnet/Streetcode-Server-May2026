using MediatR;
using FluentResults;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Update;

public record UpdateFactCommand(FactDto Fact)
    : IRequest<Result<Unit>>
{
}
