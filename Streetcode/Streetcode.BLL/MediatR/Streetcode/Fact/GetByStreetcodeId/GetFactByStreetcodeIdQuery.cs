using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Streetcode.Fact.GetByStreetcodeId;

public record GetFactByStreetcodeIdQuery(int StreetcodeId) : IRequest<Result<IEnumerable<FactDto>>>, IHasStreetcodeId;