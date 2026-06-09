using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using Repositories.Interfaces;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Media;
using Streetcode.BLL.MediatR.Media.Video.Delete;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using VideoEntity = Streetcode.DAL.Entities.Media.Video;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Video.Delete;

/// <summary>
/// Unit tests for DeleteVideoHandler.
/// </summary>
public class DeleteVideoHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoWrapperMock;
    private readonly Mock<IVideoRepository> _videoRepoMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly DeleteVideoHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteVideoHandlerTests"/> class.
    /// </summary>
    public DeleteVideoHandlerTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<VideoProfile>();
        });

        _mapper = config.CreateMapper();

        _repoWrapperMock = new Mock<IRepositoryWrapper>();
        _videoRepoMock = new Mock<IVideoRepository>();
        _loggerMock = new Mock<ILoggerService>();

        _repoWrapperMock
            .Setup(x => x.VideoRepository)
            .Returns(_videoRepoMock.Object);

        _handler = new DeleteVideoHandler(
            _repoWrapperMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    /// <summary>
    /// Should return successful Result with VideoDTO when database delete succeeds.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnVideoDto_WhenDeleteSucceeds()
    {
        const int videoId = 1;

        var command = new DeleteVideoCommand(videoId);

        var existingVideo = new VideoEntity
        {
            Id = videoId,
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
        };

        _videoRepoMock
            .Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<VideoEntity, bool>>>(),
                null))
            .ReturnsAsync(existingVideo);

        _videoRepoMock
            .Setup(r => r.Delete(It.IsAny<VideoEntity>()));

        _repoWrapperMock
            .Setup(w => w.SaveChangesAsync())
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(videoId);
        result.Value.Url.Should().Be(existingVideo.Url);

        _videoRepoMock.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<VideoEntity, bool>>>(),
                null),
            Times.Once);

        _videoRepoMock.Verify(
            r => r.Delete(existingVideo),
            Times.Once);

        _repoWrapperMock.Verify(
            w => w.SaveChangesAsync(),
            Times.Once);

        _loggerMock.Verify(
            l => l.LogError(It.IsAny<object>(), It.IsAny<string>()),
            Times.Never);
    }

    /// <summary>
    /// Should return failed Result when Video does not exist in the database.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenVideoDoesNotExist()
    {
        const int videoId = 1;

        var command = new DeleteVideoCommand(videoId);

        _videoRepoMock
            .Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<VideoEntity, bool>>>(),
                null))
            .ReturnsAsync((VideoEntity)null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        _videoRepoMock.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<VideoEntity, bool>>>(),
                null),
            Times.Once);

        _videoRepoMock.Verify(
            r => r.Delete(It.IsAny<VideoEntity>()),
            Times.Never);

        _repoWrapperMock.Verify(
            w => w.SaveChangesAsync(),
            Times.Never);

        _loggerMock.Verify(
            l => l.LogError(It.IsAny<object>(), It.IsAny<string>()),
            Times.Once);
    }

    /// <summary>
    /// Should return failed Result when SaveChangesAsync fails.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenDeleteFails()
    {
        const int videoId = 1;

        var command = new DeleteVideoCommand(videoId);

        var existingVideo = new VideoEntity
        {
            Id = videoId,
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
        };

        _videoRepoMock
            .Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<VideoEntity, bool>>>(),
                null))
            .ReturnsAsync(existingVideo);

        _videoRepoMock
            .Setup(r => r.Delete(It.IsAny<VideoEntity>()));

        _repoWrapperMock
            .Setup(w => w.SaveChangesAsync())
            .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        _videoRepoMock.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<VideoEntity, bool>>>(),
                null),
            Times.Once);

        _videoRepoMock.Verify(
            r => r.Delete(existingVideo),
            Times.Once);

        _repoWrapperMock.Verify(
            w => w.SaveChangesAsync(),
            Times.Once);

        _loggerMock.Verify(
            l => l.LogError(It.IsAny<object>(), It.IsAny<string>()),
            Times.Once);
    }
}
