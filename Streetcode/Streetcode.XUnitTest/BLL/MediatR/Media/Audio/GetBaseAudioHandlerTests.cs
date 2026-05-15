// <copyright file="GetBaseAudioHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Audio
{
    using System.Linq.Expressions;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore.Query;
    using Moq;
    using Repositories.Interfaces;
    using Streetcode.BLL.Interfaces.BlobStorage;
    using Streetcode.BLL.Interfaces.Logging;
    using Streetcode.BLL.MediatR.Media.Audio.GetBaseAudio;
    using Streetcode.DAL.Repositories.Interfaces.Base;
    using Xunit;
    using AudioEntity = Streetcode.DAL.Entities.Media.Audio;

    /// <summary>
    /// Unit tests for the <see cref="GetBaseAudioHandler"/> class, which handles retrieving a single audio file by its identifier.
    /// </summary>
    public class GetBaseAudioHandlerTests
    {
        private const int AudioId = 1;
        private const string BlobName = "audio-file.mp3";

        private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
        private readonly Mock<IBlobService> blobServiceMock;
        private readonly Mock<ILoggerService> loggerMock;
        private readonly Mock<IAudioRepository> audioRepositoryMock;
        private readonly GetBaseAudioHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetBaseAudioHandlerTests"/> class.
        /// </summary>
        public GetBaseAudioHandlerTests()
        {
            this.repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            this.audioRepositoryMock = new Mock<IAudioRepository>();
            this.blobServiceMock = new Mock<IBlobService>();
            this.loggerMock = new Mock<ILoggerService>();

            this.repositoryWrapperMock
                .Setup(w => w.AudioRepository)
                .Returns(this.audioRepositoryMock.Object);

            this.handler = new GetBaseAudioHandler(
                this.blobServiceMock.Object,
                this.repositoryWrapperMock.Object,
                this.loggerMock.Object);
        }

        /// <summary>
        /// Tests that when the audio entity exists in the repository, the handler returns a successful result containing the memory stream retrieved from blob storage.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenAudioFound_ReturnsMemoryStreamFromBlobStorage()
        {
            var query = new GetBaseAudioQuery(AudioId);
            var entity = new AudioEntity { Id = AudioId, BlobName = BlobName };
            var expectedStream = new MemoryStream(new byte[] { 1, 2, 3 });

            this.SetupGetFirstOrDefault(entity);

            this.blobServiceMock
                .Setup(b => b.FindFileInStorageAsMemoryStream(BlobName))
                .Returns(expectedStream);

            var result = await this.handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeSameAs(expectedStream);
        }

        /// <summary>
        /// Tests that when the audio entity exists in the repository, the handler calls blob storage with the correct blob name.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenAudioFound_CallsFindFileInStorageAsMemoryStreamWithCorrectBlobName()
        {
            var query = new GetBaseAudioQuery(AudioId);
            var entity = new AudioEntity { Id = AudioId, BlobName = BlobName };

            this.SetupGetFirstOrDefault(entity);

            this.blobServiceMock
                .Setup(b => b.FindFileInStorageAsMemoryStream(BlobName))
                .Returns(new MemoryStream());

            await this.handler.Handle(query, CancellationToken.None);

            this.blobServiceMock.Verify(
                b => b.FindFileInStorageAsMemoryStream(BlobName),
                Times.Once);
        }

        /// <summary>
        /// Tests that when the repository does not contain an audio entity with the requested identifier, the handler returns a failed result with the appropriate error message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenAudioNotFound_ReturnsFailResult()
        {
            var query = new GetBaseAudioQuery(AudioId);

            this.SetupGetFirstOrDefault(null);

            var result = await this.handler.Handle(query, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should()
                .Be($"Cannot find an audio with corresponding id: {AudioId}");
        }

        /// <summary>
        /// Tests that when the repository does not contain an audio entity with the requested identifier, the handler logs an error message with the appropriate details.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenAudioNotFound_LogsError()
        {
            var query = new GetBaseAudioQuery(AudioId);

            this.SetupGetFirstOrDefault(null);

            await this.handler.Handle(query, CancellationToken.None);

            this.loggerMock.Verify(
                logger => logger.LogError(
                    query,
                    $"Cannot find an audio with corresponding id: {AudioId}"),
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
