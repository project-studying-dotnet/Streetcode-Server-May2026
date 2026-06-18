using AutoMapper;
using Streetcode.BLL.DTO.Media.ArtSlidesTemplates;
using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.BLL.Mapping.Media.Images
{
    internal class ArtSlideTeamplatesProfile : Profile
    {
        public ArtSlideTeamplatesProfile()
        {
            CreateMap<StreetcodeArtSlideTemplate, StreetcodeArtSlideTemplateDto>().ReverseMap();
        }
    }
}
