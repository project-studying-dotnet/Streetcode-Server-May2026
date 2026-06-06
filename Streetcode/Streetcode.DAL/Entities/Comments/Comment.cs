using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.DAL.Entities.Comments;

public class Comment
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int? UserId { get; set; }
    public int StreetcodeId { get; set; }
    public StreetcodeContent? Streetcode { get; set; }
    public int? ParentCommentId { get; set; }
    public Comment? ParentComment { get; set; }
    public List<Comment> Replies { get; set; } = new();
}
