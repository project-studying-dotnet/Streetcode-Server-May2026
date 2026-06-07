using AutoMapper;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Streetcode.BLL.DTO.Comments;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Comments.Create;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Comments;
using Xunit;

using CommentEntity = Streetcode.DAL.Entities.Comments.Comment;
using StreetcodeContent = Streetcode.DAL.Entities.Streetcode.StreetcodeContent;

namespace Streetcode.XUnitTest.BLL.MediatR.Comments.Create;

public class CreateCommentHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ICommentRepository> _commentRepositoryMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly IMapper _mapper;
    private readonly CreateCommentHandler _handler;

    public CreateCommentHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _commentRepositoryMock = new Mock<ICommentRepository>();
        _loggerMock = new Mock<ILoggerService>();

        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(CreateCommentHandler).Assembly);
        }).CreateMapper();

        _repositoryWrapperMock.Setup(x => x.CommentRepository).Returns(_commentRepositoryMock.Object);

        _handler = new CreateCommentHandler(
            _repositoryWrapperMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    private static CreateCommentDto CreateValidDto()
    {
        return new CreateCommentDto
        {
            Text = "Test comment",
            StreetcodeId = 1,
            UserId = 10,
        };
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenCommentCreatedSuccessfully()
    {
        var dto = CreateValidDto();
        var command = new CreateCommentCommand(dto);

        SetupStreetcodeExists(true);

        _commentRepositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<CommentEntity>()))
            .ReturnsAsync((CommentEntity comment) => comment);

        _repositoryWrapperMock
            .Setup(wrapper => wrapper.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Text.Should().Be(dto.Text);
        result.Value.StreetcodeId.Should().Be(dto.StreetcodeId);
        result.Value.UserId.Should().Be(dto.UserId);

        _commentRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<CommentEntity>()), Times.Once);
        _repositoryWrapperMock.Verify(wrapper => wrapper.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _loggerMock.Verify(logger => logger.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenStreetcodeDoesNotExist()
    {
        var dto = CreateValidDto();
        var command = new CreateCommentCommand(dto);

        SetupStreetcodeExists(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be($"Streetcode with Id {dto.StreetcodeId} does not exist.");
        _loggerMock.Verify(logger => logger.LogError(command, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenParentCommentNotFound()
    {
        var dto = CreateValidDto();
        dto.ParentCommentId = 99;
        var command = new CreateCommentCommand(dto);

        SetupStreetcodeExists(true);
        SetupComments([]);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(string.Format(ErrorMessages.ParentCommentNotFound, 99));
        _loggerMock.Verify(logger => logger.LogError(command, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenParentCommentBelongsToDifferentStreetcode()
    {
        var dto = CreateValidDto();
        dto.ParentCommentId = 1;
        var command = new CreateCommentCommand(dto);

        SetupStreetcodeExists(true);
        SetupComments([new CommentEntity { Id = 1, StreetcodeId = 999, Text = "Parent" }]);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.ParentCommentMustBelongToSameStreetcode);
        _loggerMock.Verify(logger => logger.LogError(command, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesReturnsZero()
    {
        var dto = CreateValidDto();
        var command = new CreateCommentCommand(dto);

        SetupStreetcodeExists(true);

        _commentRepositoryMock
            .Setup(repo => repo.CreateAsync(It.IsAny<CommentEntity>()))
            .ReturnsAsync((CommentEntity comment) => comment);

        _repositoryWrapperMock
            .Setup(wrapper => wrapper.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.CannotSaveCommentToDatabase);
        _loggerMock.Verify(logger => logger.LogError(command, ErrorMessages.CannotSaveCommentToDatabase), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenMapperReturnsNull()
    {
        var command = new CreateCommentCommand(CreateValidDto());
        var mapperMock = new Mock<IMapper>();

        mapperMock
            .Setup(m => m.Map<CommentEntity>(It.IsAny<CreateCommentDto>()))
            .Returns((CommentEntity)null!);

        SetupStreetcodeExists(true);

        var handler = new CreateCommentHandler(_repositoryWrapperMock.Object, mapperMock.Object, _loggerMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.CannotMapEntity);
        _loggerMock.Verify(logger => logger.LogError(command, ErrorMessages.CannotMapEntity), Times.Once);
    }

    private void SetupStreetcodeExists(bool exists)
    {
        var streetcodes = exists
            ? new[] { new StreetcodeContent { Id = 1 } }.AsQueryable().BuildMock()
            : Enumerable.Empty<StreetcodeContent>().AsQueryable().BuildMock();

        _repositoryWrapperMock
            .Setup(wrapper => wrapper.StreetcodeRepository.FindAll())
            .Returns(streetcodes);
    }

    private void SetupComments(IEnumerable<CommentEntity> comments)
    {
        _commentRepositoryMock
            .Setup(repo => repo.FindAll())
            .Returns(comments.AsQueryable().BuildMock());
    }
}
