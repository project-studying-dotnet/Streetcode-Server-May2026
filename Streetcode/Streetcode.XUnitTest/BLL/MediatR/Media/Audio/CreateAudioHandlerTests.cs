// <copyright file="CreateAudioHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Audio
{
    using AutoMapper;
    using FluentAssertions;
    using Moq;
    using Repositories.Interfaces;
    using Streetcode.BLL.DTO.Media.Audio;
    using Streetcode.BLL.Interfaces.BlobStorage;
    using Streetcode.BLL.Interfaces.Logging;
    using Streetcode.BLL.Mapping.Media;
    using Streetcode.BLL.MediatR.Media.Audio.Create;
    using Streetcode.DAL.Repositories.Interfaces.Base;
    using Xunit;
    using Streetcode.BLL.Resources;
    using AudioEntity = Streetcode.DAL.Entities.Media.Audio;

    /// <summary>
    /// Unit tests for the <see cref="CreateAudioHandler"/> class, which handles the creation of audio files in the system.
    /// </summary>
    public class CreateAudioHandlerTests
    {
        private const string HashBlobName = "abc123hash";
        private const string Extension = "mp3";
        private const string BaseFormat = "base64content";
        private const string Title = "test-audio";

        private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
        private readonly IMapper mapper;
        private readonly Mock<IBlobService> blobServiceMock;
        private readonly Mock<ILoggerService> loggerMock;
        private readonly Mock<IAudioRepository> audioRepositoryMock;
        private readonly CreateAudioHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateAudioHandlerTests"/> class.
        /// </summary>
        public CreateAudioHandlerTests()
        {
            this.repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            this.audioRepositoryMock = new Mock<IAudioRepository>();
            this.mapper = new MapperConfiguration(cfg => cfg.AddProfile<AudioProfile>()).CreateMapper();
            this.blobServiceMock = new Mock<IBlobService>();
            this.loggerMock = new Mock<ILoggerService>();

            this.repositoryWrapperMock
                .Setup(w => w.AudioRepository)
                .Returns(this.audioRepositoryMock.Object);

            this.handler = new CreateAudioHandler(
                this.blobServiceMock.Object,
                this.repositoryWrapperMock.Object,
                this.mapper,
                this.loggerMock.Object);
        }

        /// <summary>
        /// Tests that when the save operation succeeds, the handler returns a successful result containing the expected <see cref="AudioDTO"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenSaveSucceeds_ReturnsOkResultWithAudioDTO()
        {
            var command = new CreateAudioCommand(CreateDto());

            this.SetupSuccessScenario();

            var result = await this.handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.BlobName.Should().Be($"{HashBlobName}.{Extension}");
            result.Value.MimeType.Should().Be("audio/mpeg");
        }

        /// <summary>
        /// Tests that when the save operation succeeds, the handler calls the <see cref="IBlobService.SaveFileInStorage"/> method with the correct parameters.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenSaveSucceeds_CallsSaveFileInStorage()
        {
            var command = new CreateAudioCommand(CreateDto());

            this.SetupSuccessScenario();

            await this.handler.Handle(command, CancellationToken.None);

            this.blobServiceMock.Verify(
                b => b.SaveFileInStorage(BaseFormat, Title, Extension),
                Times.Once);
        }

        /// <summary>
        /// Tests that when the save operation succeeds, the handler sets the BlobName property of the created <see cref="AudioEntity"/> correctly based on the returned hash from the blob service.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenSaveSucceeds_SetsBlobNameCorrectly()
        {
            var command = new CreateAudioCommand(CreateDto());
            AudioEntity? capturedEntity = null;

            this.blobServiceMock
                .Setup(b => b.SaveFileInStorage(BaseFormat, Title, Extension))
                .Returns(HashBlobName);

            this.audioRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<AudioEntity>()))
                .Callback<AudioEntity>(a => capturedEntity = a)
                .ReturnsAsync(new AudioEntity());

            this.repositoryWrapperMock
                .Setup(w => w.SaveChangesAsync())
                .ReturnsAsync(1);

            await this.handler.Handle(command, CancellationToken.None);

            capturedEntity.Should().NotBeNull();
            capturedEntity!.BlobName.Should().Be($"{HashBlobName}.{Extension}");
        }

        /// <summary>
        /// Tests that when the save operation fails (i.e., SaveChangesAsync returns 0), the handler returns a failed result with the appropriate error message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenSaveFails_ReturnsFailResult()
        {
            var command = new CreateAudioCommand(CreateDto());

            this.blobServiceMock
                .Setup(b => b.SaveFileInStorage(BaseFormat, Title, Extension))
                .Returns(HashBlobName);

            this.audioRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<AudioEntity>()))
                .ReturnsAsync(new AudioEntity());

            this.repositoryWrapperMock
                .Setup(w => w.SaveChangesAsync())
                .ReturnsAsync(0);

            var result = await this.handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be(ErrorMessages.FailedToCreateAudio);
        }

        /// <summary>
        /// Tests that when the save operation fails (i.e., SaveChangesAsync returns 0), the handler logs an error with the appropriate message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenSaveFails_LogsError()
        {
            var command = new CreateAudioCommand(CreateDto());

            this.blobServiceMock
                .Setup(b => b.SaveFileInStorage(BaseFormat, Title, Extension))
                .Returns(HashBlobName);

            this.audioRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<AudioEntity>()))
                .ReturnsAsync(new AudioEntity());

            this.repositoryWrapperMock
                .Setup(w => w.SaveChangesAsync())
                .ReturnsAsync(0);

            await this.handler.Handle(command, CancellationToken.None);

            this.loggerMock.Verify(
                logger => logger.LogError(command, ErrorMessages.FailedToCreateAudio),
                Times.Once);
        }

        private static AudioFileBaseCreateDTO CreateDto() => new AudioFileBaseCreateDTO
        {
            BaseFormat = BaseFormat,
            Title = Title,
            Extension = Extension,
            MimeType = "audio/mpeg",
            Description = "Test description",
        };

        private void SetupSuccessScenario()
        {
            this.blobServiceMock
                .Setup(b => b.SaveFileInStorage(BaseFormat, Title, Extension))
                .Returns(HashBlobName);

            this.audioRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<AudioEntity>()))
                .ReturnsAsync(new AudioEntity());

            this.repositoryWrapperMock
                .Setup(w => w.SaveChangesAsync())
                .ReturnsAsync(1);
        }
    }
}