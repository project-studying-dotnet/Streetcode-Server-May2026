// <copyright file="CreateTextHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Streetcode.XUnitTest.BLL.MediatR.StreetCode.Text.Create
{
    using System;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using FluentAssertions;
    using global::Streetcode.BLL.DTO.Streetcode.TextContent.Text;
    using global::Streetcode.BLL.Interfaces.Logging;
    using global::Streetcode.BLL.Mapping.Streetcode.TextContent;
    using global::Streetcode.BLL.MediatR.Streetcode.Text.Create;
    using global::Streetcode.DAL.Entities.Streetcode;
    using global::Streetcode.DAL.Repositories.Interfaces.Base;
    using global::Streetcode.DAL.Repositories.Interfaces.Streetcode;
    using global::Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
    using MockQueryable.Moq;
    using Moq;
    using Xunit;
    using T = global::Streetcode.DAL.Entities.Streetcode.TextContent;

    /// <summary>
    /// Unit tests for CreateTextHandler.
    /// </summary>
    public class CreateTextHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> repoWrapperMock;
        private readonly Mock<ITextRepository> textRepoMock;
        private readonly Mock<IStreetcodeRepository> streetcodeRepoMock;
        private readonly IMapper mapper;
        private readonly Mock<ILoggerService> loggerMock;

        private readonly CreateTextHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateTextHandlerTests"/> class.
        /// </summary>
        public CreateTextHandlerTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TextProfile>();
            });

            this.mapper = config.CreateMapper();

            this.repoWrapperMock = new Mock<IRepositoryWrapper>();
            this.textRepoMock = new Mock<ITextRepository>();
            this.streetcodeRepoMock = new Mock<IStreetcodeRepository>();
            this.loggerMock = new Mock<ILoggerService>();

            this.repoWrapperMock
                .Setup(x => x.TextRepository)
                .Returns(this.textRepoMock.Object);

            this.repoWrapperMock
                .Setup(x => x.StreetcodeRepository)
                .Returns(this.streetcodeRepoMock.Object);

            this.handler = new CreateTextHandler(
                this.repoWrapperMock.Object,
                this.mapper,
                this.loggerMock.Object);
        }

        /// <summary>
        /// Should return successful Result with TextDTO when database save succeeds.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnTextDto_WhenSaveSucceeds()
        {
            var requestDto = CreateRequestDto();
            var command = new CreateTextCommand(requestDto);

            var streetcodes = new List<StreetcodeContent>
            {
                new StreetcodeContent { Id = requestDto.StreetcodeId },
            }.AsQueryable().BuildMock();

            var texts = new List<T.Text>().AsQueryable().BuildMock();

            this.streetcodeRepoMock
                .Setup(r => r.FindAll())
                .Returns(streetcodes);

            this.textRepoMock
                .Setup(r => r.FindAll())
                .Returns(texts);

            this.textRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<T.Text>()))
                .ReturnsAsync((T.Text entity) => entity);

            this.repoWrapperMock
                .Setup(w => w.SaveChangesAsync())
                .ReturnsAsync(1);

            var result = await this.handler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Title.Should().Be(requestDto.Title);

            this.streetcodeRepoMock.Verify(r => r.FindAll(), Times.Once);
            this.textRepoMock.Verify(r => r.FindAll(), Times.Once);
            this.textRepoMock.Verify(r => r.CreateAsync(It.IsAny<T.Text>()), Times.Once);
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
            var requestDto = CreateRequestDto();
            var command = new CreateTextCommand(requestDto);

            var emptyStreetcodes = new List<StreetcodeContent>().AsQueryable().BuildMock();

            this.streetcodeRepoMock
                .Setup(r => r.FindAll())
                .Returns(emptyStreetcodes);

            var result = await this.handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Message.Contains($"Streetcode with Id {requestDto.StreetcodeId} does not exist."));

            this.streetcodeRepoMock.Verify(r => r.FindAll(), Times.Once);
            this.textRepoMock.Verify(r => r.FindAll(), Times.Never);
            this.textRepoMock.Verify(r => r.CreateAsync(It.IsAny<T.Text>()), Times.Never);
            this.repoWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Never);
            this.loggerMock.Verify(l => l.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Once);
        }

        /// <summary>
        /// Should return failed Result when Text for Streetcode already exists.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnFailResult_WhenTextAlreadyExists()
        {
            var requestDto = CreateRequestDto();
            var command = new CreateTextCommand(requestDto);

            var streetcodes = new List<StreetcodeContent>
            {
                new StreetcodeContent { Id = requestDto.StreetcodeId },
            }.AsQueryable().BuildMock();

            var existingTexts = new List<T.Text>
            {
                new T.Text { StreetcodeId = requestDto.StreetcodeId },
            }.AsQueryable().BuildMock();

            this.streetcodeRepoMock
                .Setup(r => r.FindAll())
                .Returns(streetcodes);

            this.textRepoMock
                .Setup(r => r.FindAll())
                .Returns(existingTexts);

            var result = await this.handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Message.Contains("already exists"));

            this.streetcodeRepoMock.Verify(r => r.FindAll(), Times.Once);
            this.textRepoMock.Verify(r => r.FindAll(), Times.Once);
            this.textRepoMock.Verify(r => r.CreateAsync(It.IsAny<T.Text>()), Times.Never);
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
            var command = new CreateTextCommand(requestDto);

            var streetcodes = new List<StreetcodeContent>
            {
                new StreetcodeContent { Id = requestDto.StreetcodeId },
            }.AsQueryable().BuildMock();

            var texts = new List<T.Text>().AsQueryable().BuildMock();

            this.streetcodeRepoMock
                .Setup(r => r.FindAll())
                .Returns(streetcodes);

            this.textRepoMock
                .Setup(r => r.FindAll())
                .Returns(texts);

            this.textRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<T.Text>()))
                .ReturnsAsync((T.Text entity) => entity);

            this.repoWrapperMock
                .Setup(w => w.SaveChangesAsync())
                .ReturnsAsync(0);

            var result = await this.handler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle();

            this.streetcodeRepoMock.Verify(r => r.FindAll(), Times.Once);
            this.textRepoMock.Verify(r => r.FindAll(), Times.Once);
            this.textRepoMock.Verify(r => r.CreateAsync(It.IsAny<T.Text>()), Times.Once);
            this.repoWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Once);
            this.loggerMock.Verify(l => l.LogError(It.IsAny<object>(), "Failed to save new Text."), Times.Once);
        }

        /// <summary>
        /// Should propagate exception when repository throws.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldPropagateException_WhenRepositoryThrows()
        {
            var requestDto = CreateRequestDto();
            var command = new CreateTextCommand(requestDto);

            var streetcodes = new List<StreetcodeContent>
            {
                new StreetcodeContent { Id = requestDto.StreetcodeId },
            }.AsQueryable().BuildMock();

            this.streetcodeRepoMock
                .Setup(r => r.FindAll())
                .Returns(streetcodes);

            this.textRepoMock
                .Setup(r => r.FindAll())
                .Throws(new Exception("Database connection failure"));

            Func<Task> act = () => this.handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Database connection failure");

            this.streetcodeRepoMock.Verify(r => r.FindAll(), Times.Once);
            this.textRepoMock.Verify(r => r.FindAll(), Times.Once);

            this.repoWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Never);
            this.loggerMock.Verify(l => l.LogError(It.IsAny<object>(), It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Checks if the handler returns failure result when mapping produces a null entity.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnFail_WhenMapperReturnsNull()
        {
            var requestDto = CreateRequestDto();
            var command = new CreateTextCommand(requestDto);

            var streetcodes = new List<StreetcodeContent>
            {
                new StreetcodeContent { Id = requestDto.StreetcodeId },
            }.AsQueryable().BuildMock();

            var texts = new List<T.Text>().AsQueryable().BuildMock();

            this.streetcodeRepoMock
                .Setup(r => r.FindAll())
                .Returns(streetcodes);

            this.textRepoMock
                .Setup(r => r.FindAll())
                .Returns(texts);

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(m => m.Map<T.Text>(It.IsAny<TextCreateDto>()))
                .Returns((T.Text?)null!);

            var customHandler = new CreateTextHandler(
                this.repoWrapperMock.Object,
                mockMapper.Object,
                this.loggerMock.Object);

            var result = await customHandler.Handle(command, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Message.Contains("map"));

            this.streetcodeRepoMock.Verify(r => r.FindAll(), Times.Once);
            this.textRepoMock.Verify(r => r.FindAll(), Times.Once);
            this.textRepoMock.Verify(r => r.CreateAsync(It.IsAny<T.Text>()), Times.Never);
            this.repoWrapperMock.Verify(w => w.SaveChangesAsync(), Times.Never);

            this.loggerMock.Verify(
                l => l.LogError(It.IsAny<object>(), "Cannot map CreateTextRequest to entity."),
                Times.Once);
        }

        /// <summary>
        /// Creates valid TextCreateDTO test data.
        /// </summary>
        private static TextCreateDto CreateRequestDto() =>
           new TextCreateDto()
           {
               Title = "Test Title",
               TextContent = "Text Content",
               AdditionalText = "Additional Text",
               StreetcodeId = 4,
           };
    }
}