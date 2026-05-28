using AutoMapper;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.DAL.Entities.Timeline;

namespace Streetcode.BLL.Mapping.Timeline;

public class TimelineItemProfile : Profile
{
    public TimelineItemProfile()
    {
        CreateMap<TimelineItem, TimelineItemDto>()
            .ForMember(dest => dest.HistoricalContexts, opt => opt.MapFrom(src => src.HistoricalContextTimelines));

        CreateMap<TimelineItemDto, TimelineItem>()
            .ForMember(dest => dest.HistoricalContextTimelines, opt => opt.Ignore())
            .ForMember(dest => dest.Streetcode, opt => opt.Ignore());

        CreateMap<HistoricalContextTimeline, HistoricalContextDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.HistoricalContextId))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.HistoricalContext!.Title));
    }
}
