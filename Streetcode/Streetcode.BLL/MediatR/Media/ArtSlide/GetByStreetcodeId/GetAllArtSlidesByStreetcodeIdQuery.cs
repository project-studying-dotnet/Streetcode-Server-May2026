using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.ArtSlides;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Media.ArtSlide.GetAllByStreetcodeId
{
    public record GetAllArtSlidesByStreetcodeIdQuery(int StreetcodeId) : IRequest<Result<IEnumerable<StreetcodeArtSlideDto>>>, IHasStreetcodeId;
}