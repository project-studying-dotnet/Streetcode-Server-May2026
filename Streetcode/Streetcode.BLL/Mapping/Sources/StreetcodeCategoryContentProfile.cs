using AutoMapper;
using Streetcode.BLL.DTO.Sources;
using Streetcode.DAL.Entities.Sources;

namespace Streetcode.BLL.Mapping.Sources
{
    internal class StreetcodeCategoryContentProfile : Profile
    {
        public StreetcodeCategoryContentProfile()
        {
            CreateMap<StreetcodeCategoryContent, StreetcodeCategoryContentDto>().ReverseMap();
            CreateMap<CategoryContentCreateDto, StreetcodeCategoryContent>().ReverseMap();
            CreateMap<CategoryContentUpdateDto, StreetcodeCategoryContent>().ReverseMap();
        }
    }
}
