namespace Streetcode.BLL.DTO.Comments;

public class UpdateCommentDto
{
    public int Id { get; set; }
    required public string Text { get; set; }
    public int StreetcodeId { get; set; }
    public int? UserId { get; set; }
    public int? ParentCommentId { get; set; }
}
