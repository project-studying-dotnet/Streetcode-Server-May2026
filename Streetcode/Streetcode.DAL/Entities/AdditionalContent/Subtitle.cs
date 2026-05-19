namespace Streetcode.DAL.Entities.AdditionalContent;

public class Subtitle
{
    public int Id { get; set; }
    public string? SubtitleText { get; set; }
    public int StreetcodeId { get; set; }
    public Streetcode.StreetcodeContent? Streetcode { get; set; }
}
