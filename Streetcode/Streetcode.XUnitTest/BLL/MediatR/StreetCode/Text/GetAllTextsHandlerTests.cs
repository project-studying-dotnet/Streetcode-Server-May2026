// <copyright file="GetAllTextsHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Streetcode.XUnitTest.BLL.MediatR.StreetCode.Text
{
    using System.Linq.Expressions;
    using AutoMapper;
    using FluentAssertions;
    using global::Streetcode.BLL.DTO.Streetcode.TextContent.Text;
    using global::Streetcode.BLL.Interfaces.Logging;
    using global::Streetcode.BLL.Mapping.Streetcode.TextContent;
    using global::Streetcode.BLL.MediatR.Streetcode.Text.GetAll;
    using global::Streetcode.DAL.Repositories.Interfaces.Base;
    using global::Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
    using Microsoft.EntityFrameworkCore.Query;
    using Moq;
    using Xunit;
    using TextEntity = global::Streetcode.DAL.Entities.Streetcode.TextContent.Text;

    /// <summary>
    /// Unit tests for <see cref="GetAllTextsHandler"/>.
    /// </summary>
    public class GetAllTextsHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
        private readonly Mock<ITextRepository> textRepositoryMock;
        private readonly IMapper mapper;
        private readonly Mock<ILoggerService> loggerMock;
        private readonly GetAllTextsHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllTextsHandlerTests"/> class.
        /// </summary>
        public GetAllTextsHandlerTests()
        {
            this.repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            this.textRepositoryMock = new Mock<ITextRepository>();
            this.mapper = new MapperConfiguration(cfg => cfg.AddProfile<TextProfile>()).CreateMapper();
            this.loggerMock = new Mock<ILoggerService>();

            this.repositoryWrapperMock
                .Setup(w => w.TextRepository)
                .Returns(this.textRepositoryMock.Object);

            this.handler = new GetAllTextsHandler(
                this.repositoryWrapperMock.Object,
                this.mapper,
                this.loggerMock.Object);
        }

        /// <summary>
        /// Tests that the Handle method returns all texts when they exist in the repository.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ReturnsAllTexts_WhenTextsExist()
        {
            // Arrange
            var query = new GetAllTextsQuery();

            var texts = new List<TextEntity>
            {
                new TextEntity { Id = 1, Title = "Title 1", TextContent = "Content 1", StreetcodeId = 1 },
                new TextEntity { Id = 2, Title = "Title 2", TextContent = "Content 2", StreetcodeId = 2 },
            };

            var textDtos = new List<TextDto>
            {
                new TextDto { Id = 1, Title = "Title 1", TextContent = "Content 1", StreetcodeId = 1 },
                new TextDto { Id = 2, Title = "Title 2", TextContent = "Content 2", StreetcodeId = 2 },
            };

            this.textRepositoryMock
                .Setup(repo => repo.GetAllAsync(
                    It.IsAny<Expression<Func<TextEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TextEntity>, IIncludableQueryable<TextEntity, object>>?>()))
                .ReturnsAsync(texts);

            // Act
            var result = await this.handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(textDtos);

            this.loggerMock.Verify(
                l => l.LogError(It.IsAny<object>(), It.IsAny<string>()),
                Times.Never);
        }

        /// <summary>
        /// Tests that the Handle method returns an error when the repository returns null.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ReturnsError_WhenRepositoryReturnsNull()
        {
            // Arrange
            var query = new GetAllTextsQuery();

            this.textRepositoryMock
                .Setup(repo => repo.GetAllAsync(
                    It.IsAny<Expression<Func<TextEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TextEntity>, IIncludableQueryable<TextEntity, object>>?>()))
                .ReturnsAsync((IEnumerable<TextEntity>)null!);

            // Act
            var result = await this.handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be("Cannot find any text");

            this.loggerMock.Verify(
                l => l.LogError(query, "Cannot find any text"),
                Times.Once);
        }

        /// <summary>
        /// Tests that the Handle method calls the mapper with the correct entity when texts are retrieved from the repository.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_CallsMapper_WithCorrectEntity()
        {
            // Arrange
            var query = new GetAllTextsQuery();

            var texts = new List<TextEntity>
            {
                new TextEntity { Id = 1, Title = "Title 1", TextContent = "Content 1", StreetcodeId = 1 },
            };

            this.textRepositoryMock
                .Setup(repo => repo.GetAllAsync(
                    It.IsAny<Expression<Func<TextEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TextEntity>, IIncludableQueryable<TextEntity, object>>?>()))
                .ReturnsAsync(texts);

            // Act
            var result = await this.handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().ContainSingle(dto => dto.Id == 1 && dto.Title == "Title 1");
        }
    }
}
