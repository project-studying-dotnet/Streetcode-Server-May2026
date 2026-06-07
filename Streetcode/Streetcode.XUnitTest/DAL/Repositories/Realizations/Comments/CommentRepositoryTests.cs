using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Comments;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Comments;
using Xunit;

namespace Streetcode.XUnitTest.DAL.Repositories.Realizations.Comments;

public class CommentRepositoryTests
{
    private readonly DbContextOptions<StreetcodeDbContext> _options;

    public CommentRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<StreetcodeDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public async Task Create_ShouldAddCommentToDatabase()
    {
        // Arrange
        using var context = new StreetcodeDbContext(_options);
        var repository = new CommentRepository(context);
        var comment = new Comment { Text = "Test Comment", StreetcodeId = 1 };

        // Act
        await repository.CreateAsync(comment);
        await context.SaveChangesAsync();

        // Assert
        var savedComment = await context.Comments.FirstOrDefaultAsync(c => c.Text == "Test Comment");
        savedComment.Should().NotBeNull();
        savedComment!.Text.Should().Be("Test Comment");
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllComments()
    {
        // Arrange
        using var context = new StreetcodeDbContext(_options);
        context.Comments.AddRange(
            new Comment { Text = "Comment 1", StreetcodeId = 1 },
            new Comment { Text = "Comment 2", StreetcodeId = 1 }
        );
        await context.SaveChangesAsync();
        var repository = new CommentRepository(context);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }
}
