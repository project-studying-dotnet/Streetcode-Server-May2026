using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Text.GetById;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
using System.Linq.Expressions;
using Xunit;
using TextEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Text;

namespace Streetcode.XUnitTest.BLL.MediatR.Streetcode.Text
{
    public class GetTextByIdHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
        private readonly Mock<ITextRepository> textRepositoryMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<ILoggerService> loggerMock;
        private readonly GetTextByIdHandler handler;

        public GetTextByIdHandlerTests()
        {
            this.repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            this.textRepositoryMock = new Mock<ITextRepository>();
            this.mapperMock = new Mock<IMapper>();
            this.loggerMock = new Mock<ILoggerService>();

            this.repositoryWrapperMock
                .Setup(w => w.TextRepository)
                .Returns(this.textRepositoryMock.Object);

            this.handler = new GetTextByIdHandler(
                this.repositoryWrapperMock.Object,
                this.mapperMock.Object,
                this.loggerMock.Object);
        }

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

            this.mapperMock
                .Setup(m => m.Map<TextDTO>(textEntity))
                .Returns(textDto);

            // Act
            var result = await this.handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(textDto);

            this.loggerMock.Verify(
                l => l.LogError(It.IsAny<object>(), It.IsAny<string>()),
                Times.Never);
        }

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
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();
            result.Errors[0].Message.Should().Be($"Cannot find any text with corresponding id: {textId}");

            this.loggerMock.Verify(
                l => l.LogError(query, $"Cannot find any text with corresponding id: {textId}"),
                Times.Once);

            this.mapperMock.Verify(
                m => m.Map<TextDTO>(It.IsAny<TextEntity>()),
                Times.Never);
        }
    }
}
