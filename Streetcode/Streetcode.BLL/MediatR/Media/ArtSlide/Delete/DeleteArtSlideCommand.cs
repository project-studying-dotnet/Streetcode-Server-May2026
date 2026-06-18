using FluentResults;
using MediatR;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Media.ArtSlide.Delete
{
    public record DeleteArtSlideCommand(int Id) : IRequest<Result<Unit>>, IHasId;
}