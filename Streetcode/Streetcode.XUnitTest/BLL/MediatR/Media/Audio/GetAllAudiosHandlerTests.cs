using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Repositories.Interfaces;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Media;
using Streetcode.BLL.MediatR.Media.Audio.GetAll;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using AudioEntity = Streetcode.DAL.Entities.Media.Audio;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Audio;

public class GetAllAudiosHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly IMapper _mapper;
    private readonly Mock<IBlobService> _blobServiceMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly Mock<IAudioRepository> _audioRepositoryMock;
    private readonly GetAllAudiosHandler _handler;

    public GetAllAudiosHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _audioRepositoryMock = new Mock<IAudioRepository>();

        _mapper = new MapperConfiguration(cfg =>
            cfg.AddProfile<AudioProfile>())
            .CreateMapper();

        _blobServiceMock = new Mock<IBlobService>();
        _loggerMock = new Mock<ILoggerService>();

        _repositoryWrapperMock
            .Setup(w => w.AudioRepository)
            .Returns(_audioRepositoryMock.Object);

        _handler = new GetAllAudiosHandler(
            _repositoryWrapperMock.Object,
            _mapper,
            _blobServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenAudiosExist_ReturnsOkResultWithMappedDTOs()
    {
        var query = new GetAllAudiosQuery();

        var entities = new List<AudioEntity>
        {
            new()
            {
                Id = 1,
                BlobName = "blob1.mp3",
            },
        };

        var dtos = new List<AudioDTO>
        {
            new()
            {
                Id = 1,
                BlobName = "blob1.mp3",
                Base64 = "base64value",
            },
        };

        SetupGetAllAsync(entities);

        _blobServiceMock
            .Setup(b => b.FindFileInStorageAsBase64(It.IsAny<string>()))
            .Returns("base64value");

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(dtos);
    }

    [Fact]
    public async Task Handle_WhenAudiosExist_SetsBase64ForEachAudio()
    {
        var query = new GetAllAudiosQuery();

        var entities = new List<AudioEntity>
        {
            new()
            {
                Id = 1,
                BlobName = "blob1.mp3",
            },
            new()
            {
                Id = 2,
                BlobName = "blob2.mp3",
            },
        };

        SetupGetAllAsync(entities);

        _blobServiceMock
            .Setup(b => b.FindFileInStorageAsBase64("blob1.mp3"))
            .Returns("base64-for-blob1");

        _blobServiceMock
            .Setup(b => b.FindFileInStorageAsBase64("blob2.mp3"))
            .Returns("base64-for-blob2");

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        result.Value.ElementAt(0).Base64.Should()
            .Be("base64-for-blob1");

        result.Value.ElementAt(1).Base64.Should()
            .Be("base64-for-blob2");
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsNull_ReturnsFailResult()
    {
        var query = new GetAllAudiosQuery();

        SetupGetAllAsync(null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors[0].Message.Should()
            .Be(ErrorMessages.CannotFindAnyAudios);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsNull_LogsError()
    {
        var query = new GetAllAudiosQuery();

        SetupGetAllAsync(null);

        await _handler.Handle(query, CancellationToken.None);

        _loggerMock.Verify(
            logger => logger.LogError(
                query,
                ErrorMessages.CannotFindAnyAudios),
            Times.Once);
    }

    private void SetupGetAllAsync(List<AudioEntity>? returnValue)
    {
        _audioRepositoryMock
            .Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<AudioEntity, bool>>>(),
                It.IsAny<Func<IQueryable<AudioEntity>,
                    IIncludableQueryable<AudioEntity, object>>?>()))
            .ReturnsAsync(returnValue!);
    }
}