using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Media.ArtSlidesTemplates;

namespace Streetcode.BLL.MediatR.Media.ArtSlideTeamplates.GetAll
{
    public record GetAllArtSlideTemplatesQuery() : IRequest<Result<IEnumerable<StreetcodeArtSlideTemplateDto>>>;
}