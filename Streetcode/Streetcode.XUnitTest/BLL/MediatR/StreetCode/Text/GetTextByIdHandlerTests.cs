// <copyright file="GetTextByIdHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Streetcode.XUnitTest.BLL.MediatR.StreetCode.Text
{
    using System.Linq.Expressions;
    using AutoMapper;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore.Query;
    using Moq;
    using global::Streetcode.BLL.DTO.Streetcode.TextContent.Text;
    using global::Streetcode.BLL.Interfaces.Logging;
    using global::Streetcode.BLL.Mapping.Streetcode.TextContent;
    using global::Streetcode.BLL.MediatR.Streetcode.Text.GetById;
    using global::Streetcode.DAL.Repositories.Interfaces.Base;
    using global::Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
    using Xunit;
    using TextEntity = global::Streetcode.DAL.Entities.Streetcode.TextContent.Text;

    /// <summary>
    /// Unit tests for <see cref="GetTextByIdHandler"/>.
    /// </summary>
    public class GetTextByIdHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
        private readonly Mock<ITextRepository> textRepositoryMock;
        private readonly IMapper mapper;
        private readonly Mock<ILoggerService> loggerMock;
        private readonly GetTextByIdHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetTextByIdHandlerTests"/> class.
        /// </summary>
        public GetTextByIdHandlerTests()
        {
            this.repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            this.textRepositoryMock = new Mock<ITextRepository>();
            this.mapper = new MapperConfiguration(cfg => cfg.AddProfile<TextProfile>()).CreateMapper();
            this.loggerMock = new Mock<ILoggerService>();

            this.repositoryWrapperMock
                .Setup(w => w.TextRepository)
                .Returns(this.textRepositoryMock.Object);

            this.handler = new GetTextByIdHandler(
                this.repositoryWrapperMock.Object,
                this.mapper,
                this.loggerMock.Object);
        }

        /// <summary>
        /// Tests that the Handle method returns a TextDTO when a text with the specified ID exists in the repository.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ReturnsTextDTO_WhenTextExists()
        {
            // Arrange
            const int textId = 1;
            var query = new GetTextByIdQuery(Id: textId);

            var textEntity = new TextEntity
            {
                Id = textId,
                Title = "Test Title",
                TextContent = "Test Content",
                StreetcodeId = 1,
            };

            var textDto = new TextDTO
            {
                Id = textId,
                Title = "Test Title",
                TextContent = "Test Content",
                StreetcodeId = 1,
            };

            this.textRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<TextEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TextEntity>, IIncludableQueryable<TextEntity, object>>?>()))
                .ReturnsAsync(textEntity);

            // Act
            var result = await this.handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(textDto);

            this.loggerMock.Verify(
                l => l.LogError(It.IsAny<object>(), It.IsAny<string>()),
                Times.Never);
        }

        /// <summary>
        /// Tests that the Handle method returns an error when no text with the specified ID exists in the repository.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ReturnsError_WhenTextNotFound()
        {
            // Arrange
            const int textId = 42;
            var query = new GetTextByIdQuery(Id: textId);

            this.textRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<TextEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TextEntity>, IIncludableQueryable<TextEntity, object>>?>()))
                .ReturnsAsync((TextEntity?)null);

            // Act
            var result = await this.handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be($"Cannot find any text with corresponding id: {textId}");

            this.loggerMock.Verify(
                l => l.LogError(query, $"Cannot find any text with corresponding id: {textId}"),
                Times.Once);
        }
    }
}
