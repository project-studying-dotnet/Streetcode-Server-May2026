using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.ArtSlides;

namespace Streetcode.BLL.MediatR.Media.ArtSlide.Create
{
    public record CreateArtSlideCommand(CreateStreetcodeArtSlideDto Dto) : IRequest<Result<StreetcodeArtSlideDto>>;
}
