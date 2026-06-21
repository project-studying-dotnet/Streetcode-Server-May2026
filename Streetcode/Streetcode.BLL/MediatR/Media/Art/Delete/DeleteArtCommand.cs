using FluentResults;
using MediatR;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Media.Art.Delete
{
    public record DeleteArtCommand(int Id) : IRequest<Result<Unit>>, IHasId;
}