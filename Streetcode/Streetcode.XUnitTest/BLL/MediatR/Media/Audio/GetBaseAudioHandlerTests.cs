using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Repositories.Interfaces;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Audio.GetBaseAudio;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using AudioEntity = Streetcode.DAL.Entities.Media.Audio;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Audio;

public class GetBaseAudioHandlerTests
{
    private const int AudioId = 1;
    private const string BlobName = "audio-file.mp3";

    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IBlobService> _blobServiceMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly Mock<IAudioRepository> _audioRepositoryMock;
    private readonly GetBaseAudioHandler _handler;

    public GetBaseAudioHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _audioRepositoryMock = new Mock<IAudioRepository>();
        _blobServiceMock = new Mock<IBlobService>();
        _loggerMock = new Mock<ILoggerService>();

        _repositoryWrapperMock
            .Setup(w => w.AudioRepository)
            .Returns(_audioRepositoryMock.Object);

        _handler = new GetBaseAudioHandler(
            _blobServiceMock.Object,
            _repositoryWrapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenAudioFound_ReturnsMemoryStreamFromBlobStorage()
    {
        var query = new GetBaseAudioQuery(AudioId);
        var entity = new AudioEntity { Id = AudioId, BlobName = BlobName };
        var expectedStream = new MemoryStream(new byte[] { 1, 2, 3 });

        SetupGetFirstOrDefault(entity);

        _blobServiceMock
            .Setup(b => b.FindFileInStorageAsMemoryStream(BlobName))
            .Returns(expectedStream);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(expectedStream);
    }

    [Fact]
    public async Task Handle_WhenAudioFound_CallsFindFileInStorageAsMemoryStreamWithCorrectBlobName()
    {
        var query = new GetBaseAudioQuery(AudioId);
        var entity = new AudioEntity { Id = AudioId, BlobName = BlobName };

        SetupGetFirstOrDefault(entity);

        _blobServiceMock
            .Setup(b => b.FindFileInStorageAsMemoryStream(BlobName))
            .Returns(new MemoryStream());

        await _handler.Handle(query, CancellationToken.None);

        _blobServiceMock.Verify(
            b => b.FindFileInStorageAsMemoryStream(BlobName),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAudioNotFound_ReturnsFailResult()
    {
        var query = new GetBaseAudioQuery(AudioId);

        SetupGetFirstOrDefault(null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .Be(string.Format(ErrorMessages.CannotFindAudioByCategoryId, AudioId));
    }

    [Fact]
    public async Task Handle_WhenAudioNotFound_LogsError()
    {
        var query = new GetBaseAudioQuery(AudioId);

        SetupGetFirstOrDefault(null);

        await _handler.Handle(query, CancellationToken.None);

        _loggerMock.Verify(
            logger => logger.LogError(
                query,
                string.Format(ErrorMessages.CannotFindAudioByCategoryId, AudioId)),
            Times.Once);
    }

    private void SetupGetFirstOrDefault(AudioEntity? returnValue)
    {
        _audioRepositoryMock
            .Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<AudioEntity, bool>>>(),
                It.IsAny<Func<IQueryable<AudioEntity>,
                    IIncludableQueryable<AudioEntity, object>>?>()))
            .ReturnsAsync(returnValue!);
    }
}