using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Repositories.Interfaces;
using Streetcode.BLL.DTO.Media.Video;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Video.GetByStreetcodeId;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Streetcode;
using Xunit;
using StreetcodeContentEntity = Streetcode.DAL.Entities.Streetcode.StreetcodeContent;
using VideoEntity = Streetcode.DAL.Entities.Media.Video;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Video.GetByStreetcodeId;

public class GetVideoByStreetcodeIdHandlerTests
{
    private const int StreetcodeId = 1;
    private const int VideoId = 10;
    private const string VideoDescription = "Test video";

    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IVideoRepository> _videoRepoMock;
    private readonly Mock<IStreetcodeRepository> _streetcodeRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetVideoByStreetcodeIdHandler _handler;

    public GetVideoByStreetcodeIdHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _videoRepoMock = new Mock<IVideoRepository>();
        _streetcodeRepositoryMock = new Mock<IStreetcodeRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILoggerService>();

        _repositoryWrapperMock
            .Setup(wrapper => wrapper.VideoRepository)
            .Returns(_videoRepoMock.Object);

        _repositoryWrapperMock
            .Setup(wrapper => wrapper.StreetcodeRepository)
            .Returns(_streetcodeRepositoryMock.Object);

        _handler = new GetVideoByStreetcodeIdHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnVideo_WhenVideoExists()
    {
        var query = new GetVideoByStreetcodeIdQuery(StreetcodeId);

        var video = new VideoEntity
        {
            Id = VideoId,
            StreetcodeId = StreetcodeId,
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
                It.IsAny<Func<IQueryable<VideoEntity>, IIncludableQueryable<VideoEntity, object>>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(video);

        _mapperMock
            .Setup(mapper => mapper.Map<VideoDto>(video))
            .Returns(videoDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(videoDto);

        _streetcodeRepositoryMock.Verify(
            repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeContentEntity, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeContentEntity>, IIncludableQueryable<StreetcodeContentEntity, object>>?>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _loggerMock.Verify(
            logger => logger.LogError(
                It.IsAny<object>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenVideoAndStreetcodeDoNotExist()
    {
        var query = new GetVideoByStreetcodeIdQuery(StreetcodeId);

        _videoRepoMock
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<VideoEntity, bool>>>(),
                It.IsAny<Func<IQueryable<VideoEntity>, IIncludableQueryable<VideoEntity, object>>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((VideoEntity)null!);

        _streetcodeRepositoryMock
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeContentEntity, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeContentEntity>, IIncludableQueryable<StreetcodeContentEntity, object>>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((StreetcodeContentEntity)null!);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors.Should()
            .ContainSingle(error =>
                error.Message.Contains(StreetcodeId.ToString()));

        _loggerMock.Verify(
            logger => logger.LogError(
                query,
                It.Is<string>(message => message.Contains(StreetcodeId.ToString()))),
            Times.Once);

        _mapperMock.Verify(
            mapper => mapper.Map<VideoDto>(It.IsAny<VideoEntity>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnNullValue_WhenVideoDoesNotExistButStreetcodeExists()
    {
        var query = new GetVideoByStreetcodeIdQuery(StreetcodeId);

        var streetcode = new StreetcodeContentEntity
        {
            Id = StreetcodeId,
            Title = "Streetcode",
            TransliterationUrl = "streetcode",
        };

        _videoRepoMock
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<VideoEntity, bool>>>(),
                It.IsAny<Func<IQueryable<VideoEntity>, IIncludableQueryable<VideoEntity, object>>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((VideoEntity)null!);

        _streetcodeRepositoryMock
            .Setup(repository => repository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeContentEntity, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeContentEntity>, IIncludableQueryable<StreetcodeContentEntity, object>>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(streetcode);

        _mapperMock
            .Setup(mapper => mapper.Map<VideoDto>((VideoEntity)null!))
            .Returns((VideoDto)null!);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();

        _loggerMock.Verify(
            logger => logger.LogError(
                It.IsAny<object>(),
                It.IsAny<string>()),
            Times.Never);
    }
}
