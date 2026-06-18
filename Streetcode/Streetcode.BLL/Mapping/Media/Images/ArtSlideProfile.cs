using AutoMapper;
using Streetcode.BLL.DTO.Media.ArtSlides;
using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.BLL.Mapping.Media.Images
{
    public class ArtSlideProfile : Profile
    {
        public ArtSlideProfile()
        {
            CreateMap<CreateStreetcodeArtSlideDto, StreetcodeArtSlide>()
                .ForMember(dest => dest.ArtSlideItems, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<StreetcodeArtSlide, StreetcodeArtSlideDto>().ReverseMap();

            CreateMap<ArtSlideItemDto, ArtSlideItem>().ReverseMap();

            CreateMap<StreetcodeArtSlide, StreetcodeArtSlideDto>().ReverseMap();
        }
    }
}