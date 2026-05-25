using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;

namespace Streetcode.BLL.MediatR.Streetcode.Term.Create
{
    public record CreateTermCommand(CreateTermDto Term) : IRequest<Result<TermDto>>
    {
    }
}
