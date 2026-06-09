using AutoMapper;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.DAL.Entities.Timeline;

namespace Streetcode.BLL.Mapping.Timeline;

public sealed class HistoricalContextTimelineProfile : Profile
{
    public HistoricalContextTimelineProfile()
    {
        base.CreateMap<HistoricalContextTimeline, HistoricalContext>().ConvertUsing(hct => hct.HistoricalContext!);
        base.CreateMap<HistoricalContextTimeline, TimelineItem>().ConvertUsing(hct => hct.Timeline!);
        base.CreateMap<HistoricalContextTimeline, HistoricalContextDto>().ConvertUsing(
            static(HistoricalContextTimeline hct, HistoricalContextDto dto, ResolutionContext ctx) =>
            {
                return ctx.Mapper.Map<HistoricalContext, HistoricalContextDto>(hct.HistoricalContext!);
            }
        );
        base.CreateMap<HistoricalContextTimeline, TimelineItemDto>().ConvertUsing(
            static(HistoricalContextTimeline hct, TimelineItemDto dto, ResolutionContext ctx) =>
            {
                return ctx.Mapper.Map<TimelineItem, TimelineItemDto>(hct.Timeline!);
            }
        );
    }
}