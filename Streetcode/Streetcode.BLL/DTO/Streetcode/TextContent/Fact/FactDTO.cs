namespace Streetcode.BLL.DTO.Streetcode.TextContent.Fact;

public class FactDto
{
    public int Id { get; set; }
    required public string Title { get; set; }
    public int Index { get; set; }
    public int ImageId { get; set; }
    public string? ImageDescription { get; set; }
    required public string FactContent { get; set; }
    public int StreetcodeId { get; set; }
}
