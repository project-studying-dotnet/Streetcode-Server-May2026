using Streetcode.DAL.Enums;

namespace Streetcode.BLL.DTO.Timeline;

public class TimelineItemDto
{
    public int Id { get; set; }
    required public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public DateViewPattern DateViewPattern { get; set; }
    public IEnumerable<HistoricalContextDTO> HistoricalContexts { get; set; } = Enumerable.Empty<HistoricalContextDTO>();
}
