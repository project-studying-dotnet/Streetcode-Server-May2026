// <copyright file="GetAudioByIdHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Audio
{
    using System.Linq.Expressions;
    using AutoMapper;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore.Query;
    using Moq;
    using Repositories.Interfaces;
    using Streetcode.BLL.DTO.Media.Audio;
    using Streetcode.BLL.Interfaces.BlobStorage;
    using Streetcode.BLL.Interfaces.Logging;
    using Streetcode.BLL.MediatR.Media.Audio.GetById;
    using Streetcode.DAL.Repositories.Interfaces.Base;
    using Xunit;
    using AudioEntity = Streetcode.DAL.Entities.Media.Audio;

    /// <summary>
    /// Unit tests for the <see cref="GetAudioByIdHandler"/> class, which handles retrieving a single audio file by its identifier.
    /// </summary>
    public class GetAudioByIdHandlerTests
    {
        private const int AudioId = 1;
        private const string BlobName = "audio-file.mp3";
        private const string Base64Value = "base64encodedcontent";

        private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<IBlobService> blobServiceMock;
        private readonly Mock<ILoggerService> loggerMock;
        private readonly Mock<IAudioRepository> audioRepositoryMock;
        private readonly GetAudioByIdHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAudioByIdHandlerTests"/> class.
        /// </summary>
        public GetAudioByIdHandlerTests()
        {
            this.repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            this.audioRepositoryMock = new Mock<IAudioRepository>();
            this.mapperMock = new Mock<IMapper>();
            this.blobServiceMock = new Mock<IBlobService>();
            this.loggerMock = new Mock<ILoggerService>();

            this.repositoryWrapperMock
                .Setup(w => w.AudioRepository)
                .Returns(this.audioRepositoryMock.Object);

            this.handler = new GetAudioByIdHandler(
                this.repositoryWrapperMock.Object,
                this.mapperMock.Object,
                this.blobServiceMock.Object,
                this.loggerMock.Object);
        }

        /// <summary>
        /// Tests that when the audio entity is found in the repository, the handler returns a successful result containing the mapped <see cref="AudioDTO"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenAudioFound_ReturnsOkResultWithDTO()
        {
            var query = new GetAudioByIdQuery(AudioId);
            var entity = new AudioEntity { Id = AudioId, BlobName = BlobName };
            var dto = new AudioDTO { Id = AudioId, BlobName = BlobName };

            this.SetupGetFirstOrDefault(entity);

            this.mapperMock
                .Setup(m => m.Map<AudioDTO>(entity))
                .Returns(dto);

            this.blobServiceMock
                .Setup(b => b.FindFileInStorageAsBase64(BlobName))
                .Returns(Base64Value);

            var result = await this.handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(dto);
        }

        /// <summary>
        /// Tests that when the audio entity is found in the repository, the handler sets the <see cref="AudioDTO.Base64"/> property using the blob service response.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenAudioFound_SetsBase64FromBlobService()
        {
            var query = new GetAudioByIdQuery(AudioId);
            var entity = new AudioEntity { Id = AudioId, BlobName = BlobName };
            var dto = new AudioDTO { Id = AudioId, BlobName = BlobName };

            this.SetupGetFirstOrDefault(entity);

            this.mapperMock
                .Setup(m => m.Map<AudioDTO>(entity))
                .Returns(dto);

            this.blobServiceMock
                .Setup(b => b.FindFileInStorageAsBase64(BlobName))
                .Returns(Base64Value);

            var result = await this.handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Base64.Should().Be(Base64Value);
        }

        /// <summary>
        /// Tests that when the audio entity is not found in the repository, the handler returns a failed result with the appropriate error message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenAudioNotFound_ReturnsFailResult()
        {
            var query = new GetAudioByIdQuery(AudioId);

            this.SetupGetFirstOrDefault(null);

            var result = await this.handler.Handle(query, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should()
                .Be($"Cannot find an audio with corresponding id: {AudioId}");
        }

        /// <summary>
        /// Tests that when the audio entity is not found in the repository, the handler logs an error with the corresponding identifier.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenAudioNotFound_LogsError()
        {
            var query = new GetAudioByIdQuery(AudioId);

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
