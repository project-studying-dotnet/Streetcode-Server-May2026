using MediatR;
using FluentResults;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Delete
{
    public record DeleteTextCommand(int Id)
        : IRequest<Result<TextDto>>;
}
