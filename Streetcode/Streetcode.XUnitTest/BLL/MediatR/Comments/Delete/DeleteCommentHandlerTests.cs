using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Comments.Delete;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Comments;
using Xunit;

using CommentEntity = Streetcode.DAL.Entities.Comments.Comment;

namespace Streetcode.XUnitTest.BLL.MediatR.Comments.Delete;

public class DeleteCommentHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ICommentRepository> _commentRepositoryMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly DeleteCommentHandler _handler;

    public DeleteCommentHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _commentRepositoryMock = new Mock<ICommentRepository>();
        _loggerMock = new Mock<ILoggerService>();

        _repositoryWrapperMock.Setup(x => x.CommentRepository).Returns(_commentRepositoryMock.Object);

        _handler = new DeleteCommentHandler(_repositoryWrapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenCommentWithNoRepliesIsDeleted()
    {
        var comment = new CommentEntity { Id = 1, Text = "Comment", StreetcodeId = 1 };
        var command = new DeleteCommentCommand(1);

        SetupCommentLookup(comment);
        SetupGetAllComments([comment]);
        _repositoryWrapperMock
            .Setup(w => w.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        _commentRepositoryMock.Verify(r => r.Delete(comment), Times.Once);
        _commentRepositoryMock.Verify(r => r.DeleteRange(It.IsAny<IEnumerable<CommentEntity>>()), Times.Never);
        _loggerMock.Verify(l => l.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_AndDeleteRepliesFirst_WhenCommentHasDirectReplies()
    {
        var parent = new CommentEntity { Id = 1, Text = "Parent", StreetcodeId = 1 };
        var reply = new CommentEntity { Id = 2, Text = "Reply", StreetcodeId = 1, ParentCommentId = 1 };
        var command = new DeleteCommentCommand(1);

        SetupCommentLookup(parent);
        SetupGetAllComments([parent, reply]);
        _repositoryWrapperMock
            .Setup(w => w.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _commentRepositoryMock.Verify(
            r => r.DeleteRange(It.Is<IEnumerable<CommentEntity>>(
            list => list.Any(c => c.Id == reply.Id))), Times.Once);
        _commentRepositoryMock.Verify(r => r.Delete(parent), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_AndDeleteAllDescendants_WhenCommentHasNestedReplies()
    {
        var parent = new CommentEntity { Id = 1, Text = "Parent", StreetcodeId = 1 };
        var reply = new CommentEntity { Id = 2, Text = "Reply", StreetcodeId = 1, ParentCommentId = 1 };
        var nestedReply = new CommentEntity { Id = 3, Text = "Nested reply", StreetcodeId = 1, ParentCommentId = 2 };
        var command = new DeleteCommentCommand(1);

        SetupCommentLookup(parent);
        SetupGetAllComments([parent, reply, nestedReply]);
        _repositoryWrapperMock
            .Setup(w => w.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _commentRepositoryMock.Verify(
            r => r.DeleteRange(It.Is<IEnumerable<CommentEntity>>(
            list => list.Any(c => c.Id == nestedReply.Id) && list.Any(c => c.Id == reply.Id))), Times.Once);
        _commentRepositoryMock.Verify(r => r.Delete(parent), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotDeleteSiblings_WhenDeletingOneTopLevelComment()
    {
        var commentA = new CommentEntity { Id = 1, Text = "Comment A", StreetcodeId = 1 };
        var commentB = new CommentEntity { Id = 2, Text = "Comment B", StreetcodeId = 1 };
        var command = new DeleteCommentCommand(1);

        SetupCommentLookup(commentA);
        SetupGetAllComments([commentA, commentB]);
        _repositoryWrapperMock
            .Setup(w => w.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _commentRepositoryMock.Verify(r => r.Delete(commentA), Times.Once);
        _commentRepositoryMock.Verify(r => r.Delete(commentB), Times.Never);
        _commentRepositoryMock.Verify(
            r => r.DeleteRange(It.Is<IEnumerable<CommentEntity>>(
            list => list.Any(c => c.Id == commentB.Id))), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenCommentDoesNotExist()
    {
        var command = new DeleteCommentCommand(99);

        SetupCommentLookup();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(string.Format(ErrorMessages.CommentWithIdNotFound, 99));
        _commentRepositoryMock.Verify(r => r.Delete(It.IsAny<CommentEntity>()), Times.Never);
        _commentRepositoryMock.Verify(r => r.DeleteRange(It.IsAny<IEnumerable<CommentEntity>>()), Times.Never);
        _loggerMock.Verify(l => l.LogError(command, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesReturnsZero()
    {
        var comment = new CommentEntity { Id = 1, Text = "Comment", StreetcodeId = 1 };
        var command = new DeleteCommentCommand(1);

        SetupCommentLookup(comment);
        SetupGetAllComments([comment]);
        _repositoryWrapperMock
            .Setup(w => w.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.FailedToDeleteComment);
        _loggerMock.Verify(l => l.LogError(command, ErrorMessages.FailedToDeleteComment), Times.Once);
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
