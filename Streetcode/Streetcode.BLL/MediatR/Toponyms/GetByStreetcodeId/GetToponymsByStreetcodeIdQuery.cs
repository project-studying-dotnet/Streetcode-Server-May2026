using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Toponyms;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Toponyms.GetByStreetcodeId;

public record GetToponymsByStreetcodeIdQuery(int StreetcodeId) : IRequest<Result<IEnumerable<ToponymDTO>>>, IHasStreetcodeId;