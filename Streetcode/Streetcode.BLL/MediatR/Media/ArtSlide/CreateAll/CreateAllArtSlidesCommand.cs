using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.ArtSlides;

namespace Streetcode.BLL.MediatR.Media.ArtSlide.CreateAll
{
    public record CreateAllArtSlidesCommand(List<CreateStreetcodeArtSlideDto> ArtSlides)
        : IRequest<Result<IEnumerable<StreetcodeArtSlideDto>>>;
}
