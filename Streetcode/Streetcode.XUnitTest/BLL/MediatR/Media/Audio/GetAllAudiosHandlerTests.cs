// <copyright file="GetAllAudiosHandlerTests.cs" company="PlaceholderCompany">
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
    using Streetcode.BLL.MediatR.Media.Audio.GetAll;
    using Streetcode.DAL.Repositories.Interfaces.Base;
    using Xunit;
    using AudioEntity = Streetcode.DAL.Entities.Media.Audio;

    /// <summary>
    /// Unit tests for the <see cref="GetAllAudiosHandler"/> class, which handles the retrieval of all audio files in the system.
    /// </summary>
    public class GetAllAudiosHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<IBlobService> blobServiceMock;
        private readonly Mock<ILoggerService> loggerMock;
        private readonly Mock<IAudioRepository> audioRepositoryMock;
        private readonly GetAllAudiosHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllAudiosHandlerTests"/> class.
        /// </summary>
        public GetAllAudiosHandlerTests()
        {
            this.repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            this.audioRepositoryMock = new Mock<IAudioRepository>();
            this.mapperMock = new Mock<IMapper>();
            this.blobServiceMock = new Mock<IBlobService>();
            this.loggerMock = new Mock<ILoggerService>();

            this.repositoryWrapperMock
                .Setup(w => w.AudioRepository)
                .Returns(this.audioRepositoryMock.Object);

            this.handler = new GetAllAudiosHandler(
                this.repositoryWrapperMock.Object,
                this.mapperMock.Object,
                this.blobServiceMock.Object,
                this.loggerMock.Object);
        }

        /// <summary>
        /// Tests that when audio entities exist in the repository, the handler returns a successful result containing a list of mapped <see cref="AudioDTO"/> objects with their Base64 values set correctly.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenAudiosExist_ReturnsOkResultWithMappedDTOs()
        {
            var query = new GetAllAudiosQuery();
            var entities = new List<AudioEntity> { new AudioEntity { Id = 1 } };
            var dtos = new List<AudioDTO> { new AudioDTO { Id = 1, BlobName = "blob1.mp3" } };

            this.SetupGetAllAsync(entities);

            this.mapperMock
                .Setup(m => m.Map<IEnumerable<AudioDTO>>(entities))
                .Returns(dtos);

            this.blobServiceMock
                .Setup(b => b.FindFileInStorageAsBase64(It.IsAny<string>()))
                .Returns("base64value");

            var result = await this.handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(dtos);
        }

        /// <summary>
        /// Tests that when audio entities exist in the repository, the handler sets the Base64 property for each mapped <see cref="AudioDTO"/> by calling the blob service with the correct blob names.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenAudiosExist_SetsBase64ForEachAudio()
        {
            var query = new GetAllAudiosQuery();
            var entities = new List<AudioEntity>
            {
                new AudioEntity { Id = 1 },
                new AudioEntity { Id = 2 },
            };
            var dtos = new List<AudioDTO>
            {
                new AudioDTO { Id = 1, BlobName = "blob1.mp3" },
                new AudioDTO { Id = 2, BlobName = "blob2.mp3" },
            };

            this.SetupGetAllAsync(entities);

            this.mapperMock
                .Setup(m => m.Map<IEnumerable<AudioDTO>>(entities))
                .Returns(dtos);

            this.blobServiceMock
                .Setup(b => b.FindFileInStorageAsBase64("blob1.mp3"))
                .Returns("base64-for-blob1");

            this.blobServiceMock
                .Setup(b => b.FindFileInStorageAsBase64("blob2.mp3"))
                .Returns("base64-for-blob2");

            var result = await this.handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.ElementAt(0).Base64.Should().Be("base64-for-blob1");
            result.Value.ElementAt(1).Base64.Should().Be("base64-for-blob2");
        }

        /// <summary>
        /// Tests that when the repository returns null (indicating no audio entities found), the handler returns a failed result with the appropriate error message.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenRepositoryReturnsNull_ReturnsFailResult()
        {
            var query = new GetAllAudiosQuery();

            this.SetupGetAllAsync(null);

            var result = await this.handler.Handle(query, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be("Cannot find any audios");
        }

        /// <summary>
        /// Tests that when the repository returns null (indicating no audio entities found), the handler logs an error message with the appropriate details.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_WhenRepositoryReturnsNull_LogsError()
        {
            var query = new GetAllAudiosQuery();

            this.SetupGetAllAsync(null);

            await this.handler.Handle(query, CancellationToken.None);

            this.loggerMock.Verify(
                logger => logger.LogError(query, "Cannot find any audios"),
                Times.Once);
        }

        private void SetupGetAllAsync(IEnumerable<AudioEntity>? returnValue)
        {
            this.audioRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<AudioEntity, bool>>?>(),
                    It.IsAny<Func<IQueryable<AudioEntity>, IIncludableQueryable<AudioEntity, object>>?>()))
                .ReturnsAsync(returnValue);
        }
    }
}
