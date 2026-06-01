using AutoMapper;
using Streetcode.BLL.DTO.Streetcode.TextContent;
using Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm;
using Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.BLL.Mapping.Streetcode.TextContent;

public class RelatedTermProfile : Profile
{
    public RelatedTermProfile()
    {
        CreateMap<CreateRelatedTermDTO, RelatedTerm>().ReverseMap();

        CreateMap<RelatedTerm, RelatedTermDTO>().ReverseMap();
     }
}
