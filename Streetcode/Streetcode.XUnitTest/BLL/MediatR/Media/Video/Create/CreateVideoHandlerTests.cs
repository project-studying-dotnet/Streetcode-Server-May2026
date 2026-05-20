// <copyright file="CreateVideoHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Video.Create
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentAssertions;
    using MockQueryable.Moq;
    using Moq;
    using Repositories.Interfaces;
    using Streetcode.BLL.DTO.Media.Video;
    using Streetcode.BLL.Interfaces.Logging;
    using Streetcode.BLL.Mapping.Media;
    using Streetcode.BLL.MediatR.Media.Video.Create;
    using Streetcode.DAL.Entities.Streetcode;
    using Streetcode.DAL.Repositories.Interfaces.Base;
    using Streetcode.DAL.Repositories.Interfaces.Media;
    using Streetcode.DAL.Repositories.Interfaces.Streetcode;
    using Xunit;
    using T = Streetcode.DAL.Entities.Media;

    /// <summary>
    /// Unit tests for CreateVideoHandler.
    /// </summary>
    public class CreateVideoHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> repoWrapperMock;
        private readonly Mock<IVideoRepository> videoRepoMock;
        private readonly Mock<IStreetcodeRepository> streetcodeRepoMock;
        private readonly IMapper mapper;
        private readonly Mock<ILoggerService> loggerMock;
        private readonly CreateVideoHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateVideoHandlerTests"/> class.
        /// </summary>
        public CreateVideoHandlerTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<VideoProfile>();
            });

            this.mapper = config.CreateMapper();

            this.repoWrapperMock = new Mock<IRepositoryWrapper>();
            this.videoRepoMock = new Mock<IVideoRepository>();
            this.streetcodeRepoMock = new Mock<IStreetcodeRepository>();
            this.loggerMock = new Mock<ILoggerService>();

            this.repoWrapperMock
                .Setup(x => x.VideoRepository)
                .Returns(this.videoRepoMock.Object);

            this.repoWrapperMock
                .Setup(x => x.StreetcodeRepository)
                .Returns(this.streetcodeRepoMock.Object);

            this.handler = new CreateVideoHandler(
                this.repoWrapperMock.Object,
                this.mapper,
                this.loggerMock.Object);
        }

        /// <summary>
        /// Should return successful Result with VideoDTO when database save succeeds.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnVideoDto_WhenSaveSucceeds()
        {
            // Arrange
            var requestDto = CreateRequestDto();
            var command = new CreateVideoCommand(requestDto);

            var streetcodes = new List<StreetcodeContent>
            {
                new StreetcodeContent { Id = requestDto.StreetcodeId },
            }.AsQueryable().BuildMock();

            this.streetcodeRepoMock
                .Setup(r => r.FindAll())
                .Returns(streetcodes);

            this.videoRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<T.Video>()))
                .ReturnsAsync((T.Video entity) => entity);

            this.repoWrapperMock
                .Setup(w => w.SaveChangesAsync())
                .ReturnsAsync(1);

            var result = await this.handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Url.Should().Be(requestDto.Url);

            this.streetcodeRepoMock.Verify(r => r.FindAll(), Times.Once);
            this.videoRepoMock.Verify(r => r.CreateAsync(It.IsAny<T.Video>()), Times.Once);
            this.repoWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Once);

            this.loggerMock.Verify(
                l => l.LogError(It.IsAny<object>(), It.IsAny<string>()),
                Times.Never);
        }

        /// <summary>
        /// Should return failed Result when Streetcode does not exist.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnFailResult_WhenStreetcodeDoesNotExist()
        {
            // Arrange
            var requestDto = CreateRequestDto();
            var command = new CreateVideoCommand(requestDto);

            var emptyStreetcodes = new List<StreetcodeContent>().AsQueryable().BuildMock();

            this.streetcodeRepoMock
                .Setup(r => r.FindAll())
                .Returns(emptyStreetcodes);

            var result = await this.handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Message.Contains($"Streetcode with Id {requestDto.StreetcodeId} does not exist."));

            this.streetcodeRepoMock.Verify(r => r.FindAll(), Times.Once);
            this.videoRepoMock.Verify(r => r.CreateAsync(It.IsAny<T.Video>()), Times.Never);
            this.repoWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Never);
            this.loggerMock.Verify(l => l.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Should return failed Result and log error when database save fails.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnFailResult_WhenSaveFails()
        {
            var requestDto = CreateRequestDto();
            var command = new CreateVideoCommand(requestDto);

            var streetcodes = new List<StreetcodeContent>
            {
                new StreetcodeContent { Id = requestDto.StreetcodeId },
            }.AsQueryable().BuildMock();

            this.streetcodeRepoMock
                .Setup(r => r.FindAll())
                .Returns(streetcodes);

            this.videoRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<T.Video>()))
                .ReturnsAsync((T.Video entity) => entity);

            this.repoWrapperMock
                .Setup(w => w.SaveChangesAsync())
                .ReturnsAsync(0);

            var result = await this.handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Message.Contains("Failed to save new Video."));

            this.streetcodeRepoMock.Verify(r => r.FindAll(), Times.Once);
            this.videoRepoMock.Verify(r => r.CreateAsync(It.IsAny<T.Video>()), Times.Once);
            this.repoWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Once);
            this.loggerMock.Verify(l => l.LogError(It.IsAny<object>(), "Failed to save new Video."), Times.Once);
        }

        /// <summary>
        /// Checks if the handler returns failure result when mapping produces a null entity.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnFail_WhenMapperReturnsNull()
        {
            var requestDto = CreateRequestDto();
            var command = new CreateVideoCommand(requestDto);

            var streetcodes = new List<StreetcodeContent>
            {
                new StreetcodeContent { Id = requestDto.StreetcodeId },
            }.AsQueryable().BuildMock();

            this.streetcodeRepoMock
                .Setup(r => r.FindAll())
                .Returns(streetcodes);

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(m => m.Map<T.Video>(It.IsAny<VideoCreateDTO>()))
                .Returns((T.Video?)null!);

            var customHandler = new CreateVideoHandler(
                this.repoWrapperMock.Object,
                mockMapper.Object,
                this.loggerMock.Object);

            var result = await customHandler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Message.Contains("Cannot map CreateVideoRequest to entity."));

            this.streetcodeRepoMock.Verify(r => r.FindAll(), Times.Once);
            this.videoRepoMock.Verify(r => r.CreateAsync(It.IsAny<T.Video>()), Times.Never);
            this.repoWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Never);
            this.loggerMock.Verify(l => l.LogError(It.IsAny<object>(), "Cannot map CreateVideoRequest to entity."), Times.Once);
        }

        /// <summary>
        /// Creates valid VideoCreateDTO test data.
        /// </summary>
        private static VideoCreateDTO CreateRequestDto() =>
           new VideoCreateDTO()
           {
               Url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
               StreetcodeId = 4,
           };
    }
}