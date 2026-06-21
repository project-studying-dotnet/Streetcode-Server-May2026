using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Comments;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Comments.GetRepliesByCommentId;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Comments;
using Xunit;

using CommentEntity = Streetcode.DAL.Entities.Comments.Comment;

namespace Streetcode.XUnitTest.BLL.MediatR.Comments.GetRepliesByCommentId;

public class GetRepliesByCommentIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ICommentRepository> _commentRepositoryMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly IMapper _mapper;
    private readonly GetRepliesByCommentIdHandler _handler;

    public GetRepliesByCommentIdHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _commentRepositoryMock = new Mock<ICommentRepository>();
        _loggerMock = new Mock<ILoggerService>();

        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(GetRepliesByCommentIdHandler).Assembly);
        }).CreateMapper();

        _repositoryWrapperMock.Setup(x => x.CommentRepository).Returns(_commentRepositoryMock.Object);

        _handler = new GetRepliesByCommentIdHandler(
            _repositoryWrapperMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WithReplies_WhenParentCommentExists()
    {
        var query = new GetRepliesByCommentIdQuery(1);
        var parentComment = new CommentEntity { Id = 1, Text = "Parent", StreetcodeId = 1 };
        var replies = new List<CommentEntity>
        {
            new() { Id = 2, Text = "Reply A", StreetcodeId = 1, ParentCommentId = 1, CreatedAt = DateTime.UtcNow.AddMinutes(-2) },
            new() { Id = 3, Text = "Reply B", StreetcodeId = 1, ParentCommentId = 1, CreatedAt = DateTime.UtcNow.AddMinutes(-1) },
        };
        var allComments = new List<CommentEntity> { parentComment }.Concat(replies).ToList();

        SetupCommentLookup(parentComment);
        SetupGetAllComments(allComments);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        _loggerMock.Verify(l => l.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WithEmptyList_WhenNoRepliesExist()
    {
        var query = new GetRepliesByCommentIdQuery(1);
        var parentComment = new CommentEntity { Id = 1, Text = "Parent", StreetcodeId = 1 };

        SetupCommentLookup(parentComment);
        SetupGetAllComments([parentComment]);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
        _loggerMock.Verify(l => l.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WithRepliesOrderedChronologically()
    {
        var query = new GetRepliesByCommentIdQuery(1);
        var parentComment = new CommentEntity { Id = 1, Text = "Parent", StreetcodeId = 1 };
        var replies = new List<CommentEntity>
        {
            new() { Id = 3, Text = "Later reply", StreetcodeId = 1, ParentCommentId = 1, CreatedAt = DateTime.UtcNow.AddMinutes(-1) },
            new() { Id = 2, Text = "Earlier reply", StreetcodeId = 1, ParentCommentId = 1, CreatedAt = DateTime.UtcNow.AddMinutes(-5) },
        };

        SetupCommentLookup(parentComment);
        SetupGetAllComments(replies);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var ordered = result.Value.ToList();
        ordered[0].Text.Should().Be("Earlier reply");
        ordered[1].Text.Should().Be("Later reply");
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenParentCommentDoesNotExist()
    {
        var query = new GetRepliesByCommentIdQuery(99);

        SetupCommentLookup();

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(string.Format(ErrorMessages.CommentWithIdNotFound, 99));
        _loggerMock.Verify(l => l.LogError(query, It.IsAny<string>()), Times.Once);
    }

    private void SetupCommentLookup(params CommentEntity[] comments)
    {
        _commentRepositoryMock
            .Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<CommentEntity, bool>>>(),
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((
                Expression<Func<CommentEntity, bool>> predicate,
                Func<IQueryable<CommentEntity>, IIncludableQueryable<CommentEntity, object>>? _,
                CancellationToken __) =>
                comments.FirstOrDefault(predicate.Compile()));
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
