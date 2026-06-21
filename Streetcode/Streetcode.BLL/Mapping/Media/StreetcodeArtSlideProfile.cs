using AutoMapper;
using Streetcode.BLL.DTO.Media.ArtSlides;
using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.BLL.Mapping.Media
{
    public class StreetcodeArtSlideProfile : Profile
    {
        public StreetcodeArtSlideProfile()
        {
            CreateMap<StreetcodeArtSlide, StreetcodeArtSlideDto>()
                .ForMember(dto => dto.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dto => dto.Template, opt => opt.MapFrom(src => src.TemplateId))
                .ReverseMap()
                .ForMember(src => src.Id, opt => opt.MapFrom(dto => dto.Id))
                .ForMember(src => src.TemplateId, opt => opt.MapFrom(dto => dto.Template));
        }
    }
}