using System.Linq.Expressions;
using FluentAssertions;
using global::MediatR;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Repositories.Interfaces;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Audio.Delete;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using AudioEntity = Streetcode.DAL.Entities.Media.Audio;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Audio;

public class DeleteAudioHandlerTests
{
    private const int AudioId = 1;
    private const string BlobName = "audio-file.mp3";

    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IBlobService> _blobServiceMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly Mock<IAudioRepository> _audioRepositoryMock;
    private readonly DeleteAudioHandler _handler;

    public DeleteAudioHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _audioRepositoryMock = new Mock<IAudioRepository>();
        _blobServiceMock = new Mock<IBlobService>();
        _loggerMock = new Mock<ILoggerService>();

        _repositoryWrapperMock
            .Setup(w => w.AudioRepository)
            .Returns(_audioRepositoryMock.Object);

        _handler = new DeleteAudioHandler(
            _repositoryWrapperMock.Object,
            _blobServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenAudioNotFound_ReturnsFailResult()
    {
        var command = new DeleteAudioCommand(AudioId);

        SetupGetFirstOrDefault(null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors[0].Message.Should()
            .Be(string.Format(ErrorMessages.CannotFindAudioByCategoryId, AudioId));
    }

    [Fact]
    public async Task Handle_WhenAudioNotFound_LogsError()
    {
        var command = new DeleteAudioCommand(AudioId);

        SetupGetFirstOrDefault(null);

        await _handler.Handle(command, CancellationToken.None);

        _loggerMock.Verify(
            logger => logger.LogError(
                command,
                string.Format(ErrorMessages.CannotFindAudioByCategoryId, AudioId)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSaveSucceeds_ReturnsOkResult()
    {
        var audio = new AudioEntity
        {
            Id = AudioId,
            BlobName = BlobName,
        };

        var command = new DeleteAudioCommand(AudioId);

        SetupGetFirstOrDefault(audio);

        _repositoryWrapperMock
            .Setup(w => w.SaveChangesAsync())
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task Handle_WhenSaveSucceeds_CallsDeleteFileInStorage()
    {
        var audio = new AudioEntity
        {
            Id = AudioId,
            BlobName = BlobName,
        };

        var command = new DeleteAudioCommand(AudioId);

        SetupGetFirstOrDefault(audio);

        _repositoryWrapperMock
            .Setup(w => w.SaveChangesAsync())
            .ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        _blobServiceMock.Verify(
            b => b.DeleteFileInStorage(BlobName),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSaveFails_ReturnsFailResult()
    {
        var audio = new AudioEntity
        {
            Id = AudioId,
            BlobName = BlobName,
        };

        var command = new DeleteAudioCommand(AudioId);

        SetupGetFirstOrDefault(audio);

        _repositoryWrapperMock
            .Setup(w => w.SaveChangesAsync())
            .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors[0].Message.Should()
            .Be(ErrorMessages.FailedToDeleteAnAudio);
    }

    [Fact]
    public async Task Handle_WhenSaveFails_DoesNotCallDeleteFileInStorage()
    {
        var audio = new AudioEntity
        {
            Id = AudioId,
            BlobName = BlobName,
        };

        var command = new DeleteAudioCommand(AudioId);

        SetupGetFirstOrDefault(audio);

        _repositoryWrapperMock
            .Setup(w => w.SaveChangesAsync())
            .ReturnsAsync(0);

        await _handler.Handle(command, CancellationToken.None);

        _blobServiceMock.Verify(
            b => b.DeleteFileInStorage(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenSaveFails_LogsError()
    {
        var audio = new AudioEntity
        {
            Id = AudioId,
            BlobName = BlobName,
        };

        var command = new DeleteAudioCommand(AudioId);

        SetupGetFirstOrDefault(audio);

        _repositoryWrapperMock
            .Setup(w => w.SaveChangesAsync())
            .ReturnsAsync(0);

        await _handler.Handle(command, CancellationToken.None);

        _loggerMock.Verify(
            logger => logger.LogError(
                command,
                ErrorMessages.FailedToDeleteAnAudio),
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