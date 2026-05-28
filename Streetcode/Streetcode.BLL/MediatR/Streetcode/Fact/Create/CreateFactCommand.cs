using MediatR;
using FluentResults;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.Create;

public record CreateFactCommand(FactDto Fact)
    : IRequest<Result<FactDto>>
{
}
