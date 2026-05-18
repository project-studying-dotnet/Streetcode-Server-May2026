using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Toponyms;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Toponyms.GetById;

public record GetToponymByIdQuery(int Id) : IRequest<Result<ToponymDTO>>, IHasId;
