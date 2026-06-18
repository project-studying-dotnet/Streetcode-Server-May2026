using AutoMapper;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.BLL.Mapping.Media.Images;

public class ArtProfile : Profile
{
    public ArtProfile()
    {
        CreateMap<Art, ArtDTO>().ReverseMap();
        CreateMap<ArtCreateDto, DAL.Entities.Media.Images.Art>()
            .ForMember(dest => dest.ImageId, opt => opt.MapFrom(src => src.ImageId))
            .ForMember(dest => dest.Image, opt => opt.Ignore());
        CreateMap<ArtUpdateDto, Art>();
    }
}
