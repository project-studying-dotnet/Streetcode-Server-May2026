using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Streetcode.Text.GetByStreetcodeId;

public record GetTextByStreetcodeIdQuery(int StreetcodeId)
    : IRequest<Result<TextDto?>>, IHasStreetcodeId;