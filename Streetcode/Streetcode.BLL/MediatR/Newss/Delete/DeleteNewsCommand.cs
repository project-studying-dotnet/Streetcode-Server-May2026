using FluentResults;
using MediatR;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Newss.Delete
{
    public record DeleteNewsCommand(int Id) : IRequest<Result<Unit>>, IHasId;
}
