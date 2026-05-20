using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;

namespace Streetcode.BLL.MediatR.Streetcode.Text.Update
{
    public record UpdateTextCommand(TextUpdateDTO updateTextRequest) : IRequest<Result<TextDTO>>;
}
