using AutoMapper;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.DAL.Entities.Timeline;

namespace Streetcode.BLL.Mapping.Timeline;

public sealed class HistoricalContextTimelineProfile : Profile
{
    public HistoricalContextTimelineProfile()
    {
        base.CreateMap<HistoricalContextTimeline, HistoricalContext>().BeforeMap(static(hct, ti) =>
        {
            if(hct.HistoricalContext is null)
            {
                throw new NullReferenceException($"Property '{nameof(HistoricalContextTimeline)}.{nameof(HistoricalContextTimeline.HistoricalContext)}' must not be null");
            }
        }).ConvertUsing(hct => hct.HistoricalContext!);
        base.CreateMap<HistoricalContextTimeline, TimelineItem>().BeforeMap(static(hct, ti) =>
        {
            if(hct.Timeline is null)
            {
                throw new NullReferenceException($"Property '{nameof(HistoricalContextTimeline)}.{nameof(HistoricalContextTimeline.Timeline)}' must not be null");
            }
        }).ConvertUsing(hct => hct.Timeline!);
        base.CreateMap<HistoricalContextTimeline, HistoricalContextDto>().BeforeMap(static(hct, ti) =>
        {
            if(hct.HistoricalContext is null)
            {
                throw new NullReferenceException($"Property '{nameof(HistoricalContextTimeline)}.{nameof(HistoricalContextTimeline.HistoricalContext)}' must not be null");
            }
        }).ConvertUsing(static(HistoricalContextTimeline hct, HistoricalContextDto dto, ResolutionContext ctx) =>
        {
            return ctx.Mapper.Map<HistoricalContext, HistoricalContextDto>(hct.HistoricalContext!);
        });
        base.CreateMap<HistoricalContextTimeline, TimelineItemDto>().BeforeMap(static(hct, ti) =>
        {
            if (hct.Timeline is null)
            {
                throw new NullReferenceException($"Property '{nameof(HistoricalContextTimeline)}.{nameof(HistoricalContextTimeline.Timeline)}' must not be null");
            }
        }).ConvertUsing(static(HistoricalContextTimeline hct, TimelineItemDto dto, ResolutionContext ctx) =>
        {
            return ctx.Mapper.Map<TimelineItem, TimelineItemDto>(hct.Timeline!);
        });
    }
}