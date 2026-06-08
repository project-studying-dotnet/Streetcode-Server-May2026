namespace Streetcode.BLL.DTO.Comments;

public class CommentDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UserId { get; set; }
    public int StreetcodeId { get; set; }
    public int? ParentCommentId { get; set; }
}
