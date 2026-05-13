using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Text.GetAll;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
using System.Linq.Expressions;
using Xunit;
using TextEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Text;

namespace Streetcode.XUnitTest.BLL.MediatR.Streetcode.Text
{
    public class GetAllTextsHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
        private readonly Mock<ITextRepository> textRepositoryMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<ILoggerService> loggerMock;
        private readonly GetAllTextsHandler handler;

        public GetAllTextsHandlerTests()
        {
            this.repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            this.textRepositoryMock = new Mock<ITextRepository>();
            this.mapperMock = new Mock<IMapper>();
            this.loggerMock = new Mock<ILoggerService>();

            this.repositoryWrapperMock
                .Setup(w => w.TextRepository)
                .Returns(this.textRepositoryMock.Object);

            this.handler = new GetAllTextsHandler(
                this.repositoryWrapperMock.Object,
                this.mapperMock.Object,
                this.loggerMock.Object);
        }

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

            var textDtos = new List<TextDTO>
            {
                new TextDTO { Id = 1, Title = "Title 1", TextContent = "Content 1", StreetcodeId = 1 },
                new TextDTO { Id = 2, Title = "Title 2", TextContent = "Content 2", StreetcodeId = 2 },
            };

            this.textRepositoryMock
                .Setup(repo => repo.GetAllAsync(
                    It.IsAny<Expression<Func<TextEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TextEntity>, IIncludableQueryable<TextEntity, object>>?>()))
                .ReturnsAsync(texts);

            this.mapperMock
                .Setup(m => m.Map<IEnumerable<TextDTO>>(texts))
                .Returns(textDtos);

            // Act
            var result = await this.handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(textDtos);

            this.loggerMock.Verify(
                l => l.LogError(It.IsAny<object>(), It.IsAny<string>()),
                Times.Never);
        }

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

            this.mapperMock.Verify(
                m => m.Map<IEnumerable<TextDTO>>(It.IsAny<IEnumerable<TextEntity>>()),
                Times.Never);
        }

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

            this.mapperMock
                .Setup(m => m.Map<IEnumerable<TextDTO>>(texts))
                .Returns(new List<TextDTO>());

            // Act
            await this.handler.Handle(query, CancellationToken.None);

            // Assert
            this.mapperMock.Verify(
                m => m.Map<IEnumerable<TextDTO>>(texts),
                Times.Once);
        }
    }
}
