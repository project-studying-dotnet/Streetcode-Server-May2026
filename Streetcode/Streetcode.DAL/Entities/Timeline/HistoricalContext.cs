namespace Streetcode.DAL.Entities.Timeline;

public sealed class HistoricalContext
{
    // Value properties
    public int Id { get; set; }
    public required string Title { get; set; }

    // Navigation properties
    public List<HistoricalContextTimeline> HistoricalContextTimelines { get; set; } = [];
}