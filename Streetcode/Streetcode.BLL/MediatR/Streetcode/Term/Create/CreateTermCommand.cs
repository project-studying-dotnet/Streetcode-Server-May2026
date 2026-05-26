using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;

namespace Streetcode.BLL.MediatR.Streetcode.Term.Create
{
    public record CreateTermCommand(CreateTermDto Term) : IRequest<Result<TermDto>>
    {
    }
}
