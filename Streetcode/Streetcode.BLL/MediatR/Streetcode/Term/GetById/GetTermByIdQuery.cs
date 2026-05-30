using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Streetcode.TextContent.Term;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Streetcode.Term.GetById;

public record GetTermByIdQuery(int Id) : IRequest<Result<TermDto>>, IHasId;
