using AutoMapper;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.DAL.Entities.Timeline;

namespace Streetcode.BLL.Mapping.Timeline;

public sealed class TimelineItemProfile : Profile
{
    public TimelineItemProfile()
    {
        base.CreateMap<TimelineItem, TimelineItemDto>().ForMember(
            dest => dest.HistoricalContexts,
            opt => opt.MapFrom(ti => ti.HistoricalContextTimelines)
        );
        base.CreateMap<TimelineItemDto, TimelineItem>().ForMember(
            dest => dest.HistoricalContextTimelines,
            opt => opt.Ignore()
        ).ForMember(
            dest => dest.Streetcode,
            opt => opt.Ignore()
        );
    }
}