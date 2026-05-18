using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.MediatR.Interface;

namespace Streetcode.BLL.MediatR.Media.StreetcodeArt.GetByStreetcodeId
{
  public record GetStreetcodeArtByStreetcodeIdQuery(int StreetcodeId) : IRequest<Result<IEnumerable<StreetcodeArtDTO>>>, IHasStreetcodeId;
}
