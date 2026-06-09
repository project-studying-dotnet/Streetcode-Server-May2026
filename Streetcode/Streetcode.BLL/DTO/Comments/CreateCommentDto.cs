namespace Streetcode.BLL.DTO.Comments;

public class CreateCommentDto
{
    required public string Text { get; set; }
    public int StreetcodeId { get; set; }
    public int? UserId { get; set; }
    public int? ParentCommentId { get; set; }
}
