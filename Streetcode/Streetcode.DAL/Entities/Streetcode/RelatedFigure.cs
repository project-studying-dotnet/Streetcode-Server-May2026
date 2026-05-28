namespace Streetcode.DAL.Entities.Streetcode;

public class RelatedFigure
{
    public int ObserverId { get; set; }
    public StreetcodeContent Observer { get; set; } = null!;
    public int TargetId { get; set; }
    public StreetcodeContent Target { get; set; } = null!;
}
