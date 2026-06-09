using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Comments;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Comments.Update;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Comments;
using Xunit;

using CommentEntity = Streetcode.DAL.Entities.Comments.Comment;

namespace Streetcode.XUnitTest.BLL.MediatR.Comments.Update;

public class UpdateCommentHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ICommentRepository> _commentRepositoryMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly IMapper _mapper;
    private readonly UpdateCommentHandler _handler;

    public UpdateCommentHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _commentRepositoryMock = new Mock<ICommentRepository>();
        _loggerMock = new Mock<ILoggerService>();

        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(UpdateCommentHandler).Assembly);
        }).CreateMapper();

        _repositoryWrapperMock.Setup(x => x.CommentRepository).Returns(_commentRepositoryMock.Object);

        _handler = new UpdateCommentHandler(
            _repositoryWrapperMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    private static UpdateCommentDto CreateValidDto(int id = 1, int streetcodeId = 1)
    {
        return new UpdateCommentDto
        {
            Id = id,
            Text = "Updated comment",
            StreetcodeId = streetcodeId,
            UserId = 10,
        };
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenCommentUpdatedSuccessfully()
    {
        var dto = CreateValidDto();
        var command = new UpdateCommentCommand(dto);
        var existingComment = new CommentEntity
        {
            Id = dto.Id,
            Text = "Old comment",
            StreetcodeId = dto.StreetcodeId,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
        };

        SetupCommentLookup(existingComment);

        _repositoryWrapperMock
            .Setup(wrapper => wrapper.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Text.Should().Be(dto.Text);
        result.Value.StreetcodeId.Should().Be(dto.StreetcodeId);
        existingComment.Text.Should().Be(dto.Text);
        existingComment.UpdatedAt.Should().NotBeNull();

        _commentRepositoryMock.Verify(repo => repo.Update(existingComment), Times.Once);
        _repositoryWrapperMock.Verify(wrapper => wrapper.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _loggerMock.Verify(logger => logger.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenCommentNotFound()
    {
        var dto = CreateValidDto();
        var command = new UpdateCommentCommand(dto);

        SetupCommentLookup();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(string.Format(ErrorMessages.CommentWithIdNotFound, dto.Id));
        _loggerMock.Verify(logger => logger.LogError(command, It.IsAny<string>()), Times.Once);
        _commentRepositoryMock.Verify(repo => repo.Update(It.IsAny<CommentEntity>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenStreetcodeIdDoesNotMatch()
    {
        var dto = CreateValidDto(streetcodeId: 2);
        var command = new UpdateCommentCommand(dto);
        var existingComment = new CommentEntity
        {
            Id = dto.Id,
            Text = "Old comment",
            StreetcodeId = 1,
        };

        SetupCommentLookup(existingComment);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.ChangingCommentStreetcodeIdNotAllowed);
        _loggerMock.Verify(logger => logger.LogError(command, ErrorMessages.ChangingCommentStreetcodeIdNotAllowed), Times.Once);
        _commentRepositoryMock.Verify(repo => repo.Update(It.IsAny<CommentEntity>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenParentCommentNotFound()
    {
        var dto = CreateValidDto();
        dto.ParentCommentId = 99;
        var command = new UpdateCommentCommand(dto);
        var existingComment = new CommentEntity
        {
            Id = dto.Id,
            Text = "Old comment",
            StreetcodeId = dto.StreetcodeId,
        };

        SetupCommentLookup(existingComment);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(string.Format(ErrorMessages.ParentCommentNotFound, 99));
        _loggerMock.Verify(logger => logger.LogError(command, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenParentCommentBelongsToDifferentStreetcode()
    {
        var dto = CreateValidDto();
        dto.ParentCommentId = 2;
        var command = new UpdateCommentCommand(dto);
        var existingComment = new CommentEntity
        {
            Id = dto.Id,
            Text = "Old comment",
            StreetcodeId = dto.StreetcodeId,
        };

        SetupCommentLookup(
            existingComment,
            new CommentEntity { Id = 2, StreetcodeId = 999, Text = "Parent" });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.ParentCommentMustBelongToSameStreetcode);
        _loggerMock.Verify(logger => logger.LogError(command, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenCommentIsItsOwnParent()
    {
        var dto = CreateValidDto();
        dto.ParentCommentId = dto.Id;
        var command = new UpdateCommentCommand(dto);
        var existingComment = new CommentEntity
        {
            Id = dto.Id,
            Text = "Old comment",
            StreetcodeId = dto.StreetcodeId,
        };

        SetupCommentLookup(existingComment);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.CommentCannotBeItsOwnParent);
        _loggerMock.Verify(logger => logger.LogError(command, ErrorMessages.CommentCannotBeItsOwnParent), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenParentCommentIsDirectDescendant()
    {
        var dto = CreateValidDto(id: 1);
        dto.ParentCommentId = 2;
        var command = new UpdateCommentCommand(dto);
        var existingComment = new CommentEntity
        {
            Id = 1,
            Text = "Parent comment",
            StreetcodeId = dto.StreetcodeId,
        };
        var childComment = new CommentEntity
        {
            Id = 2,
            Text = "Child comment",
            StreetcodeId = dto.StreetcodeId,
            ParentCommentId = 1,
        };

        SetupCommentLookup(existingComment, childComment);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.ParentCommentCannotBeDescendantOfComment);
        _loggerMock.Verify(logger => logger.LogError(command, ErrorMessages.ParentCommentCannotBeDescendantOfComment), Times.Once);
        _commentRepositoryMock.Verify(repo => repo.Update(It.IsAny<CommentEntity>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenParentCommentIsNestedDescendant()
    {
        var dto = CreateValidDto(id: 1);
        dto.ParentCommentId = 3;
        var command = new UpdateCommentCommand(dto);
        var existingComment = new CommentEntity
        {
            Id = 1,
            Text = "Root comment",
            StreetcodeId = dto.StreetcodeId,
        };
        var middleComment = new CommentEntity
        {
            Id = 2,
            Text = "Middle comment",
            StreetcodeId = dto.StreetcodeId,
            ParentCommentId = 1,
        };
        var nestedComment = new CommentEntity
        {
            Id = 3,
            Text = "Nested comment",
            StreetcodeId = dto.StreetcodeId,
            ParentCommentId = 2,
        };

        SetupCommentLookup(existingComment, middleComment, nestedComment);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.ParentCommentCannotBeDescendantOfComment);
        _loggerMock.Verify(logger => logger.LogError(command, ErrorMessages.ParentCommentCannotBeDescendantOfComment), Times.Once);
        _commentRepositoryMock.Verify(repo => repo.Update(It.IsAny<CommentEntity>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesReturnsZero()
    {
        var dto = CreateValidDto();
        var command = new UpdateCommentCommand(dto);
        var existingComment = new CommentEntity
        {
            Id = dto.Id,
            Text = "Old comment",
            StreetcodeId = dto.StreetcodeId,
        };

        SetupCommentLookup(existingComment);

        _repositoryWrapperMock
            .Setup(wrapper => wrapper.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.FailedToUpdateComment);
        _loggerMock.Verify(logger => logger.LogError(command, ErrorMessages.FailedToUpdateComment), Times.Once);
    }

    private void SetupCommentLookup(params CommentEntity[] comments)
    {
        _commentRepositoryMock
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<CommentEntity, bool>>>(),
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((
                Expression<Func<CommentEntity, bool>> predicate,
                Func<IQueryable<CommentEntity>, IIncludableQueryable<CommentEntity, object>>? include,
                CancellationToken cancellationToken) =>
                comments.FirstOrDefault(predicate.Compile()));
    }
}
