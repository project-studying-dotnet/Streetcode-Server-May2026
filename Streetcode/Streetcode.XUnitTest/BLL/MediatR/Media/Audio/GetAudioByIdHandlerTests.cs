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
using Streetcode.BLL.MediatR.Media.Audio.GetById;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using AudioEntity = Streetcode.DAL.Entities.Media.Audio;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Audio;

public class GetAudioByIdHandlerTests
{
    private const int AudioId = 1;
    private const string BlobName = "audio-file.mp3";
    private const string Base64Value = "base64encodedcontent";

    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly IMapper _mapper;
    private readonly Mock<IBlobService> _blobServiceMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly Mock<IAudioRepository> _audioRepositoryMock;
    private readonly GetAudioByIdHandler _handler;

    public GetAudioByIdHandlerTests()
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

        _handler = new GetAudioByIdHandler(
            _repositoryWrapperMock.Object,
            _mapper,
            _blobServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenAudioFound_ReturnsOkResultWithDTO()
    {
        var query = new GetAudioByIdQuery(AudioId);
        var entity = new AudioEntity { Id = AudioId, BlobName = BlobName };
        var dto = new AudioDTO { Id = AudioId, BlobName = BlobName, Base64 = Base64Value };

        SetupGetFirstOrDefault(entity);

        _blobServiceMock
            .Setup(b => b.FindFileInStorageAsBase64(BlobName))
            .Returns(Base64Value);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task Handle_WhenAudioFound_SetsBase64FromBlobService()
    {
        var query = new GetAudioByIdQuery(AudioId);
        var entity = new AudioEntity { Id = AudioId, BlobName = BlobName };

        SetupGetFirstOrDefault(entity);

        _blobServiceMock
            .Setup(b => b.FindFileInStorageAsBase64(BlobName))
            .Returns(Base64Value);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Base64.Should().Be(Base64Value);
    }

    [Fact]
    public async Task Handle_WhenAudioNotFound_ReturnsFailResult()
    {
        var query = new GetAudioByIdQuery(AudioId);

        SetupGetFirstOrDefault(null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors[0].Message.Should()
            .Be(string.Format(ErrorMessages.CannotFindAudioById, AudioId));
    }

    [Fact]
    public async Task Handle_WhenAudioNotFound_LogsError()
    {
        var query = new GetAudioByIdQuery(AudioId);

        SetupGetFirstOrDefault(null);

        await _handler.Handle(query, CancellationToken.None);

        _loggerMock.Verify(
            logger => logger.LogError(
                query,
                string.Format(ErrorMessages.CannotFindAudioById, AudioId)),
            Times.Once);
    }

    private void SetupGetFirstOrDefault(AudioEntity? returnValue)
    {
        _audioRepositoryMock
            .Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<AudioEntity, bool>>>(),
                It.IsAny<Func<IQueryable<AudioEntity>,
                    IIncludableQueryable<AudioEntity, object>>?>()))
            .ReturnsAsync(returnValue);
    }
}