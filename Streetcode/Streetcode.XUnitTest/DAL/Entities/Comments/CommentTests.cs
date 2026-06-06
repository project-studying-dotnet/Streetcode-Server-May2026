using FluentAssertions;
using Streetcode.DAL.Entities.Comments;
using Xunit;

namespace Streetcode.XUnitTest.DAL.Entities.Comments;

public class CommentTests
{
    [Fact]
    public void Comment_ShouldInitializeWithDefaultValues()
    {
        // Act
        var comment = new Comment();

        // Assert
        comment.Id.Should().Be(0);
        comment.Text.Should().Be(string.Empty);
        comment.CreatedAt.Should().BeBefore(DateTime.UtcNow.AddSeconds(1));
        comment.Replies.Should().NotBeNull();
        comment.Replies.Should().BeEmpty();
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
