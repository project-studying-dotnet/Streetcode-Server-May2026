using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Streetcode.TextContent.Text;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Text;
using Streetcode.BLL.MediatR.Streetcode.Text.GetByStreetcodeId;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
using System.Linq.Expressions;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Streetcode.Text.GetByStreetcodeId
{
    public class GetTextByStreetcodeIdHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
        private readonly Mock<ITextRepository> textRepositoryMock;
        private readonly Mock<IStreetcodeRepository> streetcodeRepositoryMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<ITextService> textServiceMock;
        private readonly Mock<ILoggerService> loggerMock;
        private readonly GetTextByStreetcodeIdHandler handler;

        public GetTextByStreetcodeIdHandlerTests()
        {
            this.repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            this.textRepositoryMock = new Mock<ITextRepository>();
            this.streetcodeRepositoryMock = new Mock<IStreetcodeRepository>();
            this.mapperMock = new Mock<IMapper>();
            this.textServiceMock = new Mock<ITextService>();
            this.loggerMock = new Mock<ILoggerService>();

            this.repositoryWrapperMock
                .Setup(w => w.TextRepository)
                .Returns(this.textRepositoryMock.Object);

            this.repositoryWrapperMock
                .Setup(w => w.StreetcodeRepository)
                .Returns(this.streetcodeRepositoryMock.Object);

            this.handler = new GetTextByStreetcodeIdHandler(
                this.repositoryWrapperMock.Object,
                this.mapperMock.Object,
                this.textServiceMock.Object,
                this.loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsNullResult_WhenTextNotFoundButStreetcodeExists()
        {
            // Arrange
            var query = new GetTextByStreetcodeIdQuery(StreetcodeId: 1);

            this.textRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<DAL.Entities.Streetcode.TextContent.Text, bool>>>(),
                    It.IsAny<Func<IQueryable<DAL.Entities.Streetcode.TextContent.Text>, IIncludableQueryable<DAL.Entities.Streetcode.TextContent.Text, object>>?>()))
                .ReturnsAsync((DAL.Entities.Streetcode.TextContent.Text?)null);

            this.streetcodeRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                    It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>?>()))
                .ReturnsAsync(new StreetcodeContent { Id = 1 });

            // Act
            var result = await this.handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeNull();

            this.loggerMock.Verify(
                l => l.LogError(It.IsAny<object>(), It.IsAny<string>()),
                Times.Never);

            this.textServiceMock.Verify(
                s => s.AddTermsTag(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ReturnsError_WhenStreetcodeDoesNotExist()
        {
            // Arrange
            var query = new GetTextByStreetcodeIdQuery(StreetcodeId: 1);

            this.textRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<DAL.Entities.Streetcode.TextContent.Text, bool>>>(),
                    It.IsAny<Func<IQueryable<DAL.Entities.Streetcode.TextContent.Text>, IIncludableQueryable<DAL.Entities.Streetcode.TextContent.Text, object>>?>()))
                .ReturnsAsync((DAL.Entities.Streetcode.TextContent.Text?)null);

            this.streetcodeRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                    It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>?>()))
                .ReturnsAsync((StreetcodeContent?)null);

            // Act
            var result = await this.handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();

            this.loggerMock.Verify(
                l => l.LogError(query, It.Is<string>(msg => msg.Contains("1"))),
                Times.Once);

            this.textServiceMock.Verify(
                s => s.AddTermsTag(It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_UsesAddTermsTagResult_AsTextContent()
        {
            // Arrange
            const string originalContent = "Text about streetcode";
            const string taggedContent = "<Popover><Term>streetcode</Term><Desc>description</Desc></Popover>";
            var query = new GetTextByStreetcodeIdQuery(StreetcodeId: 1);
            var textEntity = new DAL.Entities.Streetcode.TextContent.Text
            {
                Id = 1,
                StreetcodeId = 1,
                TextContent = originalContent
            };

            this.textRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<DAL.Entities.Streetcode.TextContent.Text, bool>>>(),
                    It.IsAny<Func<IQueryable<DAL.Entities.Streetcode.TextContent.Text>, IIncludableQueryable<DAL.Entities.Streetcode.TextContent.Text, object>>?>()))
                .ReturnsAsync(textEntity);

            this.textServiceMock
                .Setup(s => s.AddTermsTag(originalContent))
                .ReturnsAsync(taggedContent);

            DAL.Entities.Streetcode.TextContent.Text? capturedEntity = null;
            this.mapperMock
                .Setup(m => m.Map<TextDTO?>(It.IsAny<DAL.Entities.Streetcode.TextContent.Text>()))
                .Callback<object>(obj => capturedEntity = obj as DAL.Entities.Streetcode.TextContent.Text)
                .Returns(new TextDTO { TextContent = taggedContent });

            // Act
            var result = await this.handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            capturedEntity.Should().NotBeNull();
            capturedEntity!.TextContent.Should().Be(taggedContent);
            result.Value!.TextContent.Should().Be(taggedContent);
        }

    }
}