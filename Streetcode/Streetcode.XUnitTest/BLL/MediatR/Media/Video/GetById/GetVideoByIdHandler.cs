using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Media.Video;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Video.GetById;
using Streetcode.BLL.Resources;
using Repositories.Interfaces;
using Streetcode.DAL.Repositories.Interfaces;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using VideoEntity = global::Streetcode.DAL.Entities.Media.Video;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Video.GetById;

public class GetVideoByIdHandlerTests
{
    private const int VideoId = 1;
    private const string VideoDescription = "Test video";

    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IVideoRepository> _videoRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetVideoByIdHandler _handler;

    public GetVideoByIdHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _videoRepoMock = new Mock<IVideoRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILoggerService>();

        _repositoryWrapperMock
            .Setup(wrapper => wrapper.VideoRepository)
            .Returns(_videoRepoMock.Object);

        _handler = new GetVideoByIdHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOk_WhenVideoExists()
    {
        var query = new GetVideoByIdQuery(VideoId);

        var videoEntity = new VideoEntity
        {
            Id = VideoId,
            Description = VideoDescription,
        };

        var videoDto = new VideoDto
        {
            Id = VideoId,
            Description = VideoDescription,
        };

        _videoRepoMock
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<VideoEntity, bool>>>(),
                It.IsAny<Func<IQueryable<VideoEntity>,
                    IIncludableQueryable<VideoEntity, object>>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(videoEntity);

        _mapperMock
            .Setup(mapper => mapper.Map<VideoDto>(videoEntity))
            .Returns(videoDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(videoDto);

        _videoRepoMock.Verify(
            repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<VideoEntity, bool>>>(),
                It.IsAny<Func<IQueryable<VideoEntity>,
                    IIncludableQueryable<VideoEntity, object>>?>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _mapperMock.Verify(
            mapper => mapper.Map<VideoDto>(videoEntity),
            Times.Once);

        _loggerMock.Verify(
            logger => logger.LogError(
                It.IsAny<object>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenVideoDoesNotExist()
    {
        var query = new GetVideoByIdQuery(VideoId);

        _videoRepoMock
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<VideoEntity, bool>>>(),
                It.IsAny<Func<IQueryable<VideoEntity>,
                    IIncludableQueryable<VideoEntity, object>>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((VideoEntity)null!);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors.Should()
            .ContainSingle(error =>
                error.Message == string.Format(
                    ErrorMessages.CannotFindVideoById,
                    VideoId));

        _videoRepoMock.Verify(
            repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<VideoEntity, bool>>>(),
                It.IsAny<Func<IQueryable<VideoEntity>,
                    IIncludableQueryable<VideoEntity, object>>?>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _mapperMock.Verify(
            mapper => mapper.Map<VideoDto>(It.IsAny<VideoEntity>()),
            Times.Never);

        _loggerMock.Verify(
            logger => logger.LogError(
                query,
                It.Is<string>(message =>
                    message.Contains(VideoId.ToString()))),
            Times.Once);
    }
}