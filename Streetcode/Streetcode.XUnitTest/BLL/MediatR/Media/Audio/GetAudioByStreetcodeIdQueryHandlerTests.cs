// <copyright file="GetAudioByStreetcodeIdQueryHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Audio
{
    using System.Linq.Expressions;
    using AutoMapper;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore.Query;
    using Moq;
    using Streetcode.BLL.DTO.Media.Audio;
    using Streetcode.BLL.Interfaces.BlobStorage;
    using Streetcode.BLL.Interfaces.Logging;
    using Streetcode.BLL.MediatR.Media.Audio.GetByStreetcodeId;
    using Streetcode.DAL.Entities.Streetcode;
    using Streetcode.DAL.Repositories.Interfaces.Base;
    using Streetcode.DAL.Repositories.Interfaces.Streetcode;
    using Xunit;
    using AudioEntity = Streetcode.DAL.Entities.Media.Audio;

    /// <summary>
    /// Unit tests for the <see cref="GetAudioByStreetcodeIdQueryHandler"/> class, which handles retrieving audio data associated with a specific streetcode identifier.
    /// </summary>
    public class GetAudioByStreetcodeIdQueryHandlerTests
    {
        private const int StreetcodeId = 1;
        private const string BlobName = "audio-file.mp3";
        private const string Base64Value = "base64encodedcontent";

        private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<IBlobService> blobServiceMock;
        private readonly Mock<ILoggerService> loggerMock;
        private readonly Mock<IStreetcodeRepository> streetcodeRepositoryMock;
        private readonly GetAudioByStreetcodeIdQueryHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAudioByStreetcodeIdQueryHandlerTests"/> class.
        /// </summary>
        public GetAudioByStreetcodeIdQueryHandlerTests()
        {
            this.repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            this.streetcodeRepositoryMock = new Mock<IStreetcodeRepository>();
            this.mapperMock = new Mock<IMapper>();
            this.blobServiceMock = new Mock<IBlobService>();
            this.loggerMock = new Mock<ILoggerService>();

            this.repositoryWrapperMock
                .Setup(w => w.StreetcodeRepository)
                .Returns(this.streetcodeRepositoryMock.Object);

            this.handler = new GetAudioByStreetcodeIdQueryHandler(
                this.repositoryWrapperMock.Object,
                this.mapperMock.Object,
                this.blobServiceMock.Object,
                this.loggerMock.Object);
        }

        /// <summary>
        /// Tests that when the streetcode with the specified identifier is not found, the handler returns a failed result with the appropriate error message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenStreetcodeNotFound_ReturnsFailResult()
        {
            var query = new GetAudioByStreetcodeIdQuery(StreetcodeId);

            this.SetupGetFirstOrDefault(null);

            var result = await this.handler.Handle(query, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should()
                .Be($"Cannot find an audio with the corresponding streetcode id: {StreetcodeId}");
        }

        /// <summary>
        /// Tests that when the streetcode with the specified identifier is not found, the handler logs an error message with the appropriate details.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenStreetcodeNotFound_LogsError()
        {
            var query = new GetAudioByStreetcodeIdQuery(StreetcodeId);

            this.SetupGetFirstOrDefault(null);

            await this.handler.Handle(query, CancellationToken.None);

            this.loggerMock.Verify(
                logger => logger.LogError(
                    query,
                    $"Cannot find an audio with the corresponding streetcode id: {StreetcodeId}"),
                Times.Once);
        }

        /// <summary>
        /// Tests that when the streetcode exists and contains an audio entity, the handler returns a successful result containing the mapped <see cref="AudioDTO"/> object.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenStreetcodeFoundAndAudioExists_ReturnsOkResultWithDTO()
        {
            var query = new GetAudioByStreetcodeIdQuery(StreetcodeId);
            var audio = new AudioEntity { Id = 1, BlobName = BlobName };
            var streetcode = new StreetcodeContent { Id = StreetcodeId, Audio = audio };
            var dto = new AudioDTO { Id = 1, BlobName = BlobName };

            this.SetupGetFirstOrDefault(streetcode);

            this.mapperMock
                .Setup(m => m.Map<AudioDTO>(audio))
                .Returns(dto);

            this.blobServiceMock
                .Setup(b => b.FindFileInStorageAsBase64(BlobName))
                .Returns(Base64Value);

            var result = await this.handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(dto);
        }

        /// <summary>
        /// Tests that when the streetcode exists and contains an audio entity, the handler sets the <see cref="AudioDTO.Base64"/> property using the blob storage service.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenStreetcodeFoundAndAudioExists_SetsBase64FromBlobService()
        {
            var query = new GetAudioByStreetcodeIdQuery(StreetcodeId);
            var audio = new AudioEntity { Id = 1, BlobName = BlobName };
            var streetcode = new StreetcodeContent { Id = StreetcodeId, Audio = audio };
            var dto = new AudioDTO { Id = 1, BlobName = BlobName };

            this.SetupGetFirstOrDefault(streetcode);

            this.mapperMock
                .Setup(m => m.Map<AudioDTO>(audio))
                .Returns(dto);

            this.blobServiceMock
                .Setup(b => b.FindFileInStorageAsBase64(BlobName))
                .Returns(Base64Value);

            var result = await this.handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Base64.Should().Be(Base64Value);
        }

        /// <summary>
        /// Tests that when the streetcode exists but does not contain an audio entity, the handler returns a successful result with no value.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenStreetcodeFoundButAudioIsNull_ReturnsNullResultWithNoValue()
        {
            var query = new GetAudioByStreetcodeIdQuery(StreetcodeId);
            var streetcode = new StreetcodeContent { Id = StreetcodeId, Audio = null };

            this.SetupGetFirstOrDefault(streetcode);

            var result = await this.handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeNull();
        }

        private void SetupGetFirstOrDefault(StreetcodeContent? returnValue)
        {
            this.streetcodeRepositoryMock
                .Setup(r => r.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                    It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>?>()))
                .ReturnsAsync(returnValue);
        }
    }
}
