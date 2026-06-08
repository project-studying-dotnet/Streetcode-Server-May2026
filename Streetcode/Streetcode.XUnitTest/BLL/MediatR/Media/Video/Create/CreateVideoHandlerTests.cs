using AutoMapper;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Repositories.Interfaces;
using Streetcode.BLL.DTO.Media.Video;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Media;
using Streetcode.BLL.MediatR.Media.Video.Create;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Streetcode;
using Xunit;

using VideoEntity = Streetcode.DAL.Entities.Media.Video;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Video.Create;

public class CreateVideoHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoWrapperMock;
    private readonly Mock<IVideoRepository> _videoRepoMock;
    private readonly Mock<IStreetcodeRepository> _streetcodeRepoMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly CreateVideoHandler _handler;

    public CreateVideoHandlerTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<VideoProfile>();
        });

        _mapper = config.CreateMapper();

        _repoWrapperMock = new Mock<IRepositoryWrapper>();
        _videoRepoMock = new Mock<IVideoRepository>();
        _streetcodeRepoMock = new Mock<IStreetcodeRepository>();
        _loggerMock = new Mock<ILoggerService>();

        _repoWrapperMock
            .Setup(x => x.VideoRepository)
            .Returns(_videoRepoMock.Object);

        _repoWrapperMock
            .Setup(x => x.StreetcodeRepository)
            .Returns(_streetcodeRepoMock.Object);

        _handler = new CreateVideoHandler(
            _repoWrapperMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnVideoDto_WhenSaveSucceeds()
    {
        var requestDto = CreateRequestDto();
        var command = new CreateVideoCommand(requestDto);

        var streetcodes = new List<StreetcodeContent>
        {
            new() { Id = requestDto.StreetcodeId },
        }.AsQueryable().BuildMock();

        var emptyVideos = new List<VideoEntity>()
            .AsQueryable()
            .BuildMock();

        _streetcodeRepoMock
            .Setup(r => r.FindAll())
            .Returns(streetcodes);

        _videoRepoMock
            .Setup(r => r.FindAll())
            .Returns(emptyVideos);

        _videoRepoMock
            .Setup(r => r.CreateAsync(It.IsAny<VideoEntity>()))
            .ReturnsAsync((VideoEntity entity) => entity);

        _repoWrapperMock
            .Setup(w => w.SaveChangesAsync())
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Url.Should().Be(requestDto.Url);

        _streetcodeRepoMock.Verify(r => r.FindAll(), Times.Once);

        _videoRepoMock.Verify(r => r.FindAll(), Times.Once);

        _videoRepoMock.Verify(
            r => r.CreateAsync(It.IsAny<VideoEntity>()),
            Times.Once);

        _repoWrapperMock.Verify(
            w => w.SaveChangesAsync(),
            Times.Once);

        _loggerMock.Verify(
            l => l.LogError(It.IsAny<object>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenStreetcodeDoesNotExist()
    {
        var requestDto = CreateRequestDto();
        var command = new CreateVideoCommand(requestDto);

        var emptyStreetcodes = new List<StreetcodeContent>()
            .AsQueryable()
            .BuildMock();

        _streetcodeRepoMock
            .Setup(r => r.FindAll())
            .Returns(emptyStreetcodes);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors.Should().ContainSingle(
            e => e.Message.Contains(
                string.Format(ErrorMessages.StreetcodeWithIdNotFound, requestDto.StreetcodeId)));

        _streetcodeRepoMock.Verify(r => r.FindAll(), Times.Once);

        _videoRepoMock.Verify(r => r.FindAll(), Times.Never);

        _videoRepoMock.Verify(
            r => r.CreateAsync(It.IsAny<VideoEntity>()),
            Times.Never);

        _repoWrapperMock.Verify(
            w => w.SaveChangesAsync(),
            Times.Never);

        _loggerMock.Verify(
            l => l.LogError(It.IsAny<object>(), It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenVideoAlreadyExists()
    {
        var requestDto = CreateRequestDto();
        var command = new CreateVideoCommand(requestDto);

        var streetcodes = new List<StreetcodeContent>
        {
            new() { Id = requestDto.StreetcodeId },
        }.AsQueryable().BuildMock();

        var existingVideos = new List<VideoEntity>
        {
            new()
            {
                Id = 1,
                StreetcodeId = requestDto.StreetcodeId,
            },
        }.AsQueryable().BuildMock();

        _streetcodeRepoMock
            .Setup(r => r.FindAll())
            .Returns(streetcodes);

        _videoRepoMock
            .Setup(r => r.FindAll())
            .Returns(existingVideos);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors.Should().ContainSingle(
            e => e.Message.Contains(
                $"Video for Streetcode Id {requestDto.StreetcodeId} already exists. Cannot create a duplicate."));

        _streetcodeRepoMock.Verify(r => r.FindAll(), Times.Once);

        _videoRepoMock.Verify(r => r.FindAll(), Times.Once);

        _videoRepoMock.Verify(
            r => r.CreateAsync(It.IsAny<VideoEntity>()),
            Times.Never);

        _repoWrapperMock.Verify(
            w => w.SaveChangesAsync(),
            Times.Never);

        _loggerMock.Verify(
            l => l.LogError(It.IsAny<object>(), It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailResult_WhenSaveFails()
    {
        var requestDto = CreateRequestDto();
        var command = new CreateVideoCommand(requestDto);

        var streetcodes = new List<StreetcodeContent>
        {
            new() { Id = requestDto.StreetcodeId },
        }.AsQueryable().BuildMock();

        var emptyVideos = new List<VideoEntity>()
            .AsQueryable()
            .BuildMock();

        _streetcodeRepoMock
            .Setup(r => r.FindAll())
            .Returns(streetcodes);

        _videoRepoMock
            .Setup(r => r.FindAll())
            .Returns(emptyVideos);

        _videoRepoMock
            .Setup(r => r.CreateAsync(It.IsAny<VideoEntity>()))
            .ReturnsAsync((VideoEntity entity) => entity);

        _repoWrapperMock
            .Setup(w => w.SaveChangesAsync())
            .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors.Should().ContainSingle(
            e => e.Message.Contains("Failed to save new Video."));

        _streetcodeRepoMock.Verify(r => r.FindAll(), Times.Once);

        _videoRepoMock.Verify(r => r.FindAll(), Times.Once);

        _videoRepoMock.Verify(
            r => r.CreateAsync(It.IsAny<VideoEntity>()),
            Times.Once);

        _repoWrapperMock.Verify(
            w => w.SaveChangesAsync(),
            Times.Once);

        _loggerMock.Verify(
            l => l.LogError(It.IsAny<object>(), "Failed to save new Video."),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenMapperReturnsNull()
    {
        var requestDto = CreateRequestDto();
        var command = new CreateVideoCommand(requestDto);

        var streetcodes = new List<StreetcodeContent>
        {
            new() { Id = requestDto.StreetcodeId },
        }.AsQueryable().BuildMock();

        var emptyVideos = new List<VideoEntity>()
            .AsQueryable()
            .BuildMock();

        _streetcodeRepoMock
            .Setup(r => r.FindAll())
            .Returns(streetcodes);

        _videoRepoMock
            .Setup(r => r.FindAll())
            .Returns(emptyVideos);

        var mockMapper = new Mock<IMapper>();

        mockMapper
            .Setup(m => m.Map<VideoEntity>(It.IsAny<VideoCreateDto>()))
            .Returns((VideoEntity)null!);

        var customHandler = new CreateVideoHandler(
            _repoWrapperMock.Object,
            mockMapper.Object,
            _loggerMock.Object);

        var result = await customHandler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors.Should().ContainSingle(
            e => e.Message.Contains("Cannot map CreateVideoRequest to entity."));

        _streetcodeRepoMock.Verify(r => r.FindAll(), Times.Once);

        _videoRepoMock.Verify(r => r.FindAll(), Times.Once);

        _videoRepoMock.Verify(
            r => r.CreateAsync(It.IsAny<VideoEntity>()),
            Times.Never);

        _repoWrapperMock.Verify(
            w => w.SaveChangesAsync(),
            Times.Never);

        _loggerMock.Verify(
            l => l.LogError(
                It.IsAny<object>(),
                "Cannot map CreateVideoRequest to entity."),
            Times.Once);
    }

    private static VideoCreateDto CreateRequestDto() =>
        new()
        {
            Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
            StreetcodeId = 4,
        };
}
