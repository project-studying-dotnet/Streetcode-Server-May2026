using FluentAssertions;
using Streetcode.DAL.Entities.Comments;
using Streetcode.DAL.Entities.Streetcode;
using Xunit;

namespace Streetcode.XUnitTest.DAL.Entities.Comments;

public class CommentTests
{
    [Fact]
    public void Comment_Properties_ShouldBeSetAndRetrievedCorrectly()
    {
        // Arrange
        var comment = new Comment();
        var streetcode = new StreetcodeContent();
        var parentComment = new Comment { Id = 1 };
        var now = DateTime.UtcNow;

        // Act
        comment.UpdatedAt = now;
        comment.UserId = 10;
        comment.Streetcode = streetcode;
        comment.ParentCommentId = 1;
        comment.ParentComment = parentComment;

        // Assert
        comment.UpdatedAt.Should().Be(now);
        comment.UserId.Should().Be(10);
        comment.Streetcode.Should().Be(streetcode);
        comment.ParentCommentId.Should().Be(1);
        comment.ParentComment.Should().Be(parentComment);
    }

    [Fact]
    public void Comment_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var testText = "Test comment text";
        var streetcodeId = 1;

        // Act
        var comment = new Comment
        {
            Id = 1,
            Text = testText,
            StreetcodeId = streetcodeId
        };

        // Assert
        comment.Id.Should().Be(1);
        comment.Text.Should().Be(testText);
        comment.StreetcodeId.Should().Be(streetcodeId);
    }

    [Fact]
    public void Comment_ShouldHandleReplyHierarchy()
    {
        // Arrange
        var parentComment = new Comment { Id = 1, Text = "Parent" };
        var reply = new Comment { Id = 2, Text = "Reply", ParentComment = parentComment };

        // Act
        parentComment.Replies.Add(reply);

        // Assert
        parentComment.Replies.Should().ContainSingle();
        parentComment.Replies.First().Id.Should().Be(2);
        reply.ParentComment.Should().Be(parentComment);
    }
}
