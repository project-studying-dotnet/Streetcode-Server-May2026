// <copyright file="DeleteVideoHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Video.Delete
{
    using System;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentAssertions;
    using Moq;
    using Repositories.Interfaces;
    using Streetcode.BLL.DTO.Media.Video;
    using Streetcode.BLL.Interfaces.Logging;
    using Streetcode.BLL.Mapping.Media;
    using Streetcode.BLL.MediatR.Media.Video.Delete;
    using Streetcode.DAL.Repositories.Interfaces.Base;
    using Streetcode.DAL.Repositories.Interfaces.Media;
    using Xunit;
    using T = Streetcode.DAL.Entities.Media;

    /// <summary>
    /// Unit tests for DeleteVideoHandler.
    /// </summary>
    public class DeleteVideoHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> repoWrapperMock;
        private readonly Mock<IVideoRepository> videoRepoMock;
        private readonly IMapper mapper;
        private readonly Mock<ILoggerService> loggerMock;
        private readonly DeleteVideoHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteVideoHandlerTests"/> class.
        /// </summary>
        public DeleteVideoHandlerTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<VideoProfile>();
            });

            this.mapper = config.CreateMapper();

            this.repoWrapperMock = new Mock<IRepositoryWrapper>();
            this.videoRepoMock = new Mock<IVideoRepository>();
            this.loggerMock = new Mock<ILoggerService>();

            this.repoWrapperMock
                .Setup(x => x.VideoRepository)
                .Returns(this.videoRepoMock.Object);

            this.handler = new DeleteVideoHandler(
                this.repoWrapperMock.Object,
                this.mapper,
                this.loggerMock.Object);
        }

        /// <summary>
        /// Should return successful Result with VideoDTO when database delete succeeds.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnVideoDto_WhenDeleteSucceeds()
        {
            int videoId = 1;
            var command = new DeleteVideoCommand(videoId);
            var existingVideo = new T.Video { Id = videoId, Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ" };

            this.videoRepoMock
                .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<T.Video, bool>>>(), null))
                .ReturnsAsync(existingVideo);

            this.videoRepoMock
                .Setup(r => r.Delete(It.IsAny<T.Video>()));

            this.repoWrapperMock
                .Setup(w => w.SaveChangesAsync())
                .ReturnsAsync(1);

            var result = await this.handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(videoId);
            result.Value.Url.Should().Be(existingVideo.Url);

            this.videoRepoMock.Verify(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<T.Video, bool>>>(), null), Times.Once);
            this.videoRepoMock.Verify(r => r.Delete(existingVideo), Times.Once);
            this.repoWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Once);

            this.loggerMock.Verify(
                l => l.LogError(It.IsAny<object>(), It.IsAny<string>()),
                Times.Never);
        }

        /// <summary>
        /// Should return failed Result when Video does not exist in the database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnFailResult_WhenVideoDoesNotExist()
        {
            int videoId = 999;
            var command = new DeleteVideoCommand(videoId);

            this.videoRepoMock
                .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<T.Video, bool>>>(), null))
                .ReturnsAsync((T.Video?)null);

            var result = await this.handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Message.Contains($"Video with Id {videoId} not found."));

            this.videoRepoMock.Verify(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<T.Video, bool>>>(), null), Times.Once);
            this.videoRepoMock.Verify(r => r.Delete(It.IsAny<T.Video>()), Times.Never);
            this.repoWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Never);
            this.loggerMock.Verify(l => l.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Should return failed Result and log error when database fails to save the deletion.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnFailResult_WhenSaveFails()
        {
            int videoId = 1;
            var command = new DeleteVideoCommand(videoId);
            var existingVideo = new T.Video { Id = videoId };

            this.videoRepoMock
                .Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<T.Video, bool>>>(), null))
                .ReturnsAsync(existingVideo);

            this.videoRepoMock
                .Setup(r => r.Delete(It.IsAny<T.Video>()));

            this.repoWrapperMock
                .Setup(w => w.SaveChangesAsync())
                .ReturnsAsync(0);

            var result = await this.handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Message.Contains($"Failed to delete Video with Id {videoId}."));

            this.videoRepoMock.Verify(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<T.Video, bool>>>(), null), Times.Once);
            this.videoRepoMock.Verify(r => r.Delete(existingVideo), Times.Once);
            this.repoWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Once);
            this.loggerMock.Verify(l => l.LogError(It.IsAny<object>(), $"Failed to delete Video with Id {videoId}."), Times.Once);
        }
    }
}