// <copyright file="DeleteAudioHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Audio
{
    using System.Linq.Expressions;
    using FluentAssertions;
    using global::MediatR;
    using Microsoft.EntityFrameworkCore.Query;
    using Moq;
    using Repositories.Interfaces;
    using Streetcode.BLL.Interfaces.BlobStorage;
    using Streetcode.BLL.Interfaces.Logging;
    using Streetcode.BLL.MediatR.Media.Audio.Delete;
    using Streetcode.DAL.Repositories.Interfaces.Base;
    using Xunit;
    using AudioEntity = Streetcode.DAL.Entities.Media.Audio;

    /// <summary>
    /// Unit tests for the <see cref="DeleteAudioHandler"/> class, which handles the deletion of audio files in the system.
    /// </summary>
    public class DeleteAudioHandlerTests
    {
        private const int AudioId = 1;
        private const string BlobName = "audio-file.mp3";

        private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
        private readonly Mock<IBlobService> blobServiceMock;
        private readonly Mock<ILoggerService> loggerMock;
        private readonly Mock<IAudioRepository> audioRepositoryMock;
        private readonly DeleteAudioHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteAudioHandlerTests"/> class.
        /// </summary>
        public DeleteAudioHandlerTests()
        {
            this.repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            this.audioRepositoryMock = new Mock<IAudioRepository>();
            this.blobServiceMock = new Mock<IBlobService>();
            this.loggerMock = new Mock<ILoggerService>();

            this.repositoryWrapperMock
                .Setup(w => w.AudioRepository)
                .Returns(this.audioRepositoryMock.Object);

            this.handler = new DeleteAudioHandler(
                this.repositoryWrapperMock.Object,
                this.blobServiceMock.Object,
                this.loggerMock.Object);
        }

        /// <summary>
        /// Tests that when the audio to be deleted is not found, the handler returns a failed result with the appropriate error message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenAudioNotFound_ReturnsFailResult()
        {
            var command = new DeleteAudioCommand(AudioId);

            this.SetupGetFirstOrDefault(null);

            var result = await this.handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should()
                .Be($"Cannot find an audio with corresponding categoryId: {AudioId}");
        }

        /// <summary>
        /// Tests that when the audio to be deleted is not found, the handler logs an error with the appropriate message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenAudioNotFound_LogsError()
        {
            var command = new DeleteAudioCommand(AudioId);

            this.SetupGetFirstOrDefault(null);

            await this.handler.Handle(command, CancellationToken.None);

            this.loggerMock.Verify(
                logger => logger.LogError(
                    command,
                    $"Cannot find an audio with corresponding categoryId: {AudioId}"),
                Times.Once);
        }

        /// <summary>
        /// Tests that when the save operation succeeds (i.e., SaveChangesAsync returns a value greater than 0), the handler returns a successful result with the expected value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenSaveSucceeds_ReturnsOkResult()
        {
            var audio = new AudioEntity { Id = AudioId, BlobName = BlobName };
            var command = new DeleteAudioCommand(AudioId);

            this.SetupGetFirstOrDefault(audio);

            this.repositoryWrapperMock
                .Setup(w => w.SaveChangesAsync())
                .ReturnsAsync(1);

            var result = await this.handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(Unit.Value);
        }

        /// <summary>
        /// Tests that when the save operation succeeds, the handler calls the <see cref="IBlobService.DeleteFileInStorage"/> method with the correct blob name to delete the associated audio file from storage.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenSaveSucceeds_CallsDeleteFileInStorage()
        {
            var audio = new AudioEntity { Id = AudioId, BlobName = BlobName };
            var command = new DeleteAudioCommand(AudioId);

            this.SetupGetFirstOrDefault(audio);

            this.repositoryWrapperMock
                .Setup(w => w.SaveChangesAsync())
                .ReturnsAsync(1);

            await this.handler.Handle(command, CancellationToken.None);

            this.blobServiceMock.Verify(
                b => b.DeleteFileInStorage(BlobName),
                Times.Once);
        }

        /// <summary>
        /// Tests that when the save operation fails (i.e., SaveChangesAsync returns 0), the handler returns a failed result with the appropriate error message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenSaveFails_ReturnsFailResult()
        {
            var audio = new AudioEntity { Id = AudioId, BlobName = BlobName };
            var command = new DeleteAudioCommand(AudioId);

            this.SetupGetFirstOrDefault(audio);

            this.repositoryWrapperMock
                .Setup(w => w.SaveChangesAsync())
                .ReturnsAsync(0);

            var result = await this.handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be("Failed to delete an audio");
        }

        /// <summary>
        /// Tests that when the save operation fails, the handler does not call the <see cref="IBlobService.DeleteFileInStorage"/> method, ensuring that the audio file is not deleted from storage if the database operation fails.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenSaveFails_DoesNotCallDeleteFileInStorage()
        {
            var audio = new AudioEntity { Id = AudioId, BlobName = BlobName };
            var command = new DeleteAudioCommand(AudioId);

            this.SetupGetFirstOrDefault(audio);

            this.repositoryWrapperMock
                .Setup(w => w.SaveChangesAsync())
                .ReturnsAsync(0);

            await this.handler.Handle(command, CancellationToken.None);

            this.blobServiceMock.Verify(
                b => b.DeleteFileInStorage(It.IsAny<string>()),
                Times.Never);
        }

        /// <summary>
        /// Tests that when the save operation fails, the handler logs an error with the appropriate message, ensuring that any issues during the delete operation are properly recorded in the logs for troubleshooting and monitoring purposes.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenSaveFails_LogsError()
        {
            var audio = new AudioEntity { Id = AudioId, BlobName = BlobName };
            var command = new DeleteAudioCommand(AudioId);

            this.SetupGetFirstOrDefault(audio);

            this.repositoryWrapperMock
                .Setup(w => w.SaveChangesAsync())
                .ReturnsAsync(0);

            await this.handler.Handle(command, CancellationToken.None);

            this.loggerMock.Verify(
                logger => logger.LogError(command, "Failed to delete an audio"),
                Times.Once);
        }

        private void SetupGetFirstOrDefault(AudioEntity? returnValue)
        {
            this.audioRepositoryMock
                .Setup(r => r.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<AudioEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<AudioEntity>, IIncludableQueryable<AudioEntity, object>>?>()))
                .ReturnsAsync(returnValue);
        }
    }
}