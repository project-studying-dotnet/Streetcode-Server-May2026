using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.ArtSlides;

namespace Streetcode.BLL.MediatR.Media.ArtSlide.Update
{
    public record UpdateArtSlideCommand(UpdateArtSlideDto Dto) : IRequest<Result<Unit>>;
}