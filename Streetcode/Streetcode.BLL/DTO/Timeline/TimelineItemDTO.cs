using Streetcode.DAL.Enums;

namespace Streetcode.BLL.DTO.Timeline;

public sealed class TimelineItemDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public DateViewPattern DateViewPattern { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int? StreetcodeId { get; set; }
    public IEnumerable<HistoricalContextDto> HistoricalContexts { get; set; } = [];
}