namespace Streetcode.DAL.Entities.Timeline;

public sealed class HistoricalContextTimeline
{
    // Value properties
    public int HistoricalContextId { get; set; }
    public int TimelineId { get; set; }

    // Navigation properties
    public HistoricalContext? HistoricalContext { get; set; }
    public TimelineItem? Timeline { get; set; }
}