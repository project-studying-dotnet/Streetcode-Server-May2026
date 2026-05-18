using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Streetcode.Text.GetById;

public record GetTextByIdQuery(int Id) : IRequest<Result<TextDTO>>, IHasId;
