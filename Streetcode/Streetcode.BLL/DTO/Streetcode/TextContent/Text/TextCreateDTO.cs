namespace Streetcode.BLL.DTO.Streetcode.TextContent.Text
{
  public class TextCreateDto
  {
    public string Title { get; set; } = string.Empty;
    public string TextContent { get; set; } = string.Empty;
    public int StreetcodeId { get; set; }
    public string? AdditionalText { get; set; }
  }
}
