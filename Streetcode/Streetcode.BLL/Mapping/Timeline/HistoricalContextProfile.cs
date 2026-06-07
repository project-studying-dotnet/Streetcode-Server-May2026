using AutoMapper;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.DAL.Entities.Timeline;

namespace Streetcode.BLL.Mapping.Timeline;

public sealed class HistoricalContextProfile : Profile
{
    public HistoricalContextProfile()
    {
        base.CreateMap<HistoricalContext, HistoricalContextDto>();
        base.CreateMap<HistoricalContextDto, HistoricalContext>().ForMember(
            dest => dest.HistoricalContextTimelines,
            opt => opt.Ignore()
        );
    }
}