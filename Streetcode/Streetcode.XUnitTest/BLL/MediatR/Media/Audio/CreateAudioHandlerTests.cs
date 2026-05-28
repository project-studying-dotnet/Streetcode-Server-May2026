using AutoMapper;
using FluentAssertions;
using Moq;
using Repositories.Interfaces;
using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Media;
using Streetcode.BLL.MediatR.Media.Audio.Create;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using AudioEntity = Streetcode.DAL.Entities.Media.Audio;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Audio;

public class CreateAudioHandlerTests
{
    private const string HashBlobName = "abc123hash";
    private const string Extension = "mp3";
    private const string BaseFormat = "base64content";
    private const string Title = "test-audio";
    private const string MimeType = "audio/mpeg";

    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly IMapper _mapper;
    private readonly Mock<IBlobService> _blobServiceMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly Mock<IAudioRepository> _audioRepositoryMock;
    private readonly CreateAudioHandler _handler;

    public CreateAudioHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _audioRepositoryMock = new Mock<IAudioRepository>();
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<AudioProfile>()).CreateMapper();
        _blobServiceMock = new Mock<IBlobService>();
        _loggerMock = new Mock<ILoggerService>();

        _repositoryWrapperMock
            .Setup(w => w.AudioRepository)
            .Returns(_audioRepositoryMock.Object);

        _handler = new CreateAudioHandler(
            _blobServiceMock.Object,
            _repositoryWrapperMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenSaveSucceeds_ReturnsOkResultWithAudioDTO()
    {
        var command = new CreateAudioCommand(CreateDto());

        SetupSuccessScenario();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.BlobName.Should().Be($"{HashBlobName}.{Extension}");
        result.Value.MimeType.Should().Be(MimeType);
    }

    [Fact]
    public async Task Handle_WhenSaveSucceeds_CallsSaveFileInStorage()
    {
        var command = new CreateAudioCommand(CreateDto());

        SetupSuccessScenario();

        await _handler.Handle(command, CancellationToken.None);

        _blobServiceMock.Verify(
            b => b.SaveFileInStorage(BaseFormat, Title, Extension),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSaveSucceeds_SetsBlobNameCorrectly()
    {
        var command = new CreateAudioCommand(CreateDto());
        AudioEntity? capturedEntity = null;

        _blobServiceMock
            .Setup(b => b.SaveFileInStorage(BaseFormat, Title, Extension))
            .Returns(HashBlobName);

        _audioRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<AudioEntity>()))
            .Callback<AudioEntity>(a => capturedEntity = a)
            .ReturnsAsync(new AudioEntity());

        _repositoryWrapperMock
            .Setup(w => w.SaveChangesAsync())
            .ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        capturedEntity.Should().NotBeNull();
        capturedEntity!.BlobName.Should().Be($"{HashBlobName}.{Extension}");
    }

    [Fact]
    public async Task Handle_WhenSaveFails_ReturnsFailResult()
    {
        var command = new CreateAudioCommand(CreateDto());

        _blobServiceMock
            .Setup(b => b.SaveFileInStorage(BaseFormat, Title, Extension))
            .Returns(HashBlobName);

        _audioRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<AudioEntity>()))
            .ReturnsAsync(new AudioEntity());

        _repositoryWrapperMock
            .Setup(w => w.SaveChangesAsync())
            .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.FailedToCreateAudio);
    }

    [Fact]
    public async Task Handle_WhenSaveFails_LogsError()
    {
        var command = new CreateAudioCommand(CreateDto());

        _blobServiceMock
            .Setup(b => b.SaveFileInStorage(BaseFormat, Title, Extension))
            .Returns(HashBlobName);

        _audioRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<AudioEntity>()))
            .ReturnsAsync(new AudioEntity());

        _repositoryWrapperMock
            .Setup(w => w.SaveChangesAsync())
            .ReturnsAsync(0);

        await _handler.Handle(command, CancellationToken.None);

        _loggerMock.Verify(
            logger => logger.LogError(command, ErrorMessages.FailedToCreateAudio),
            Times.Once);
    }

    private static AudioFileBaseCreateDTO CreateDto() => new()
    {
        BaseFormat = BaseFormat,
        Title = Title,
        Extension = Extension,
        MimeType = MimeType,
        Description = "Test description",
    };

    private void SetupSuccessScenario()
    {
        _blobServiceMock
            .Setup(b => b.SaveFileInStorage(BaseFormat, Title, Extension))
            .Returns(HashBlobName);

        _audioRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<AudioEntity>()))
            .ReturnsAsync(new AudioEntity());

        _repositoryWrapperMock
            .Setup(w => w.SaveChangesAsync())
            .ReturnsAsync(1);
    }
}