using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using MockQueryable.Moq;
using Moq;
using Streetcode.BLL.DTO.Comments;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Comments.GetByStreetcodeId;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Comments;
using Xunit;

using CommentEntity = Streetcode.DAL.Entities.Comments.Comment;
using StreetcodeContent = Streetcode.DAL.Entities.Streetcode.StreetcodeContent;

namespace Streetcode.XUnitTest.BLL.MediatR.Comments.GetByStreetcodeId;

public class GetCommentsByStreetcodeIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ICommentRepository> _commentRepositoryMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly IMapper _mapper;
    private readonly GetCommentsByStreetcodeIdHandler _handler;

    public GetCommentsByStreetcodeIdHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _commentRepositoryMock = new Mock<ICommentRepository>();
        _loggerMock = new Mock<ILoggerService>();

        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(GetCommentsByStreetcodeIdHandler).Assembly);
        }).CreateMapper();

        _repositoryWrapperMock.Setup(x => x.CommentRepository).Returns(_commentRepositoryMock.Object);

        _handler = new GetCommentsByStreetcodeIdHandler(
            _repositoryWrapperMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WithTopLevelComments_WhenStreetcodeExists()
    {
        var query = new GetCommentsByStreetcodeIdQuery(1);
        var comments = new List<CommentEntity>
        {
            new() { Id = 1, Text = "First", StreetcodeId = 1, CreatedAt = DateTime.UtcNow.AddMinutes(-2) },
            new() { Id = 2, Text = "Second", StreetcodeId = 1, CreatedAt = DateTime.UtcNow.AddMinutes(-1) },
        };

        SetupStreetcodeExists(true);
        SetupGetAllComments(comments);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        _loggerMock.Verify(l => l.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WithNestedRepliesBuilt_WhenRepliesExist()
    {
        var query = new GetCommentsByStreetcodeIdQuery(1);
        var comments = new List<CommentEntity>
        {
            new() { Id = 1, Text = "Parent", StreetcodeId = 1, CreatedAt = DateTime.UtcNow.AddMinutes(-3) },
            new() { Id = 2, Text = "Reply", StreetcodeId = 1, ParentCommentId = 1, CreatedAt = DateTime.UtcNow.AddMinutes(-2) },
            new() { Id = 3, Text = "Nested reply", StreetcodeId = 1, ParentCommentId = 2, CreatedAt = DateTime.UtcNow.AddMinutes(-1) },
        };

        SetupStreetcodeExists(true);
        SetupGetAllComments(comments);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var topLevel = result.Value.ToList();
        topLevel.Should().HaveCount(1);
        topLevel[0].Text.Should().Be("Parent");
        topLevel[0].Replies.Should().HaveCount(1);
        topLevel[0].Replies.First().Text.Should().Be("Reply");
        topLevel[0].Replies.First().Replies.Should().HaveCount(1);
        topLevel[0].Replies.First().Replies.First().Text.Should().Be("Nested reply");
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WithEmptyList_WhenNoCommentsExist()
    {
        var query = new GetCommentsByStreetcodeIdQuery(1);

        SetupStreetcodeExists(true);
        SetupGetAllComments([]);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
        _loggerMock.Verify(l => l.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WithCommentsOrderedChronologically()
    {
        var query = new GetCommentsByStreetcodeIdQuery(1);
        var comments = new List<CommentEntity>
        {
            new() { Id = 2, Text = "Later", StreetcodeId = 1, CreatedAt = DateTime.UtcNow.AddMinutes(-1) },
            new() { Id = 1, Text = "Earlier", StreetcodeId = 1, CreatedAt = DateTime.UtcNow.AddMinutes(-5) },
        };

        SetupStreetcodeExists(true);
        SetupGetAllComments(comments);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var ordered = result.Value.ToList();
        ordered[0].Text.Should().Be("Earlier");
        ordered[1].Text.Should().Be("Later");
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenStreetcodeDoesNotExist()
    {
        var query = new GetCommentsByStreetcodeIdQuery(99);

        SetupStreetcodeExists(false);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(string.Format(ErrorMessages.StreetcodeWithIdNotFound, 99));
        _loggerMock.Verify(l => l.LogError(query, It.IsAny<string>()), Times.Once);
    }

    private void SetupStreetcodeExists(bool exists)
    {
        var streetcodes = exists
            ? new[] { new StreetcodeContent { Id = 1 } }.AsQueryable().BuildMock()
            : Enumerable.Empty<StreetcodeContent>().AsQueryable().BuildMock();

        _repositoryWrapperMock
            .Setup(w => w.StreetcodeRepository.FindAll())
            .Returns(streetcodes);
    }

    private void SetupGetAllComments(IEnumerable<CommentEntity> comments)
    {
        _commentRepositoryMock
            .Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<CommentEntity, bool>>>(),
                It.IsAny<Func<IQueryable<CommentEntity>, IIncludableQueryable<CommentEntity, object>>>()))
            .ReturnsAsync((
                Expression<Func<CommentEntity, bool>> predicate,
                Func<IQueryable<CommentEntity>, IIncludableQueryable<CommentEntity, object>>? _) =>
                comments.Where(predicate.Compile()));
    }
}
