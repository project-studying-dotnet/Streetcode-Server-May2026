namespace Streetcode.XUnitTest.BLL.MediatR.StreetCode.Text
{
    using System.Linq.Expressions;
    using AutoMapper;
    using FluentAssertions;
    using global::Streetcode.BLL.DTO.Streetcode.TextContent.Text;
    using global::Streetcode.BLL.Interfaces.Logging;
    using global::Streetcode.BLL.Interfaces.Text;
    using global::Streetcode.BLL.Mapping.Streetcode.TextContent;
    using global::Streetcode.BLL.MediatR.Streetcode.Text.GetByStreetcodeId;
    using global::Streetcode.DAL.Entities.Streetcode;
    using global::Streetcode.DAL.Repositories.Interfaces.Base;
    using global::Streetcode.DAL.Repositories.Interfaces.Streetcode;
    using global::Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
    using Microsoft.EntityFrameworkCore.Query;
    using Moq;
    using Xunit;
    using TextEntity = global::Streetcode.DAL.Entities.Streetcode.TextContent.Text;

    /// <summary>
    /// Unit tests for <see cref="GetTextByStreetcodeIdHandler"/>.
    /// </summary>
    public class GetTextByStreetcodeIdHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ITextRepository> _textRepositoryMock;
        private readonly Mock<IStreetcodeRepository> _streetcodeRepositoryMock;
        private readonly IMapper _mapper;
        private readonly Mock<ITextService> _textServiceMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly GetTextByStreetcodeIdHandler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetTextByStreetcodeIdHandlerTests"/> class.
        /// </summary>
        public GetTextByStreetcodeIdHandlerTests()
        {
            this._repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            this._textRepositoryMock = new Mock<ITextRepository>();
            this._streetcodeRepositoryMock = new Mock<IStreetcodeRepository>();
            this._mapper = new MapperConfiguration(cfg => cfg.AddProfile<TextProfile>()).CreateMapper();
            this._textServiceMock = new Mock<ITextService>();
            this._loggerMock = new Mock<ILoggerService>();

            this._repositoryWrapperMock
                .Setup(w => w.TextRepository)
                .Returns(this._textRepositoryMock.Object);

            this._repositoryWrapperMock
                .Setup(w => w.StreetcodeRepository)
                .Returns(this._streetcodeRepositoryMock.Object);

            this._handler = new GetTextByStreetcodeIdHandler(
                this._repositoryWrapperMock.Object,
                this._mapper,
                this._textServiceMock.Object,
                this._loggerMock.Object);
        }

        /// <summary>
        /// Tests that the Handle method returns a TextDTO with processed text content when a text associated with the specified streetcode ID exists in the repository.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ReturnsProcessedText_WhenTextExists()
        {
            // Arrange
            const string originalContent = "Text about Maidan";
            const string taggedContent = "<Popover><Term>Maidan</Term><Desc>Central square</Desc></Popover>";
            var query = new GetTextByStreetcodeIdQuery(StreetcodeId: 1);

            var textEntity = new TextEntity
            {
                Id = 1,
                StreetcodeId = 1,
                TextContent = originalContent,
            };

            this._textRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<TextEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TextEntity>, IIncludableQueryable<TextEntity, object>>?>()))
                .ReturnsAsync(textEntity);

            this._textServiceMock
                .Setup(s => s.AddTermsTag(originalContent))
                .ReturnsAsync(taggedContent);

            // Act
            var result = await this._handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.TextContent.Should().Be(taggedContent);

            this._textServiceMock.Verify(
                s => s.AddTermsTag(originalContent),
                Times.Once);

            this._loggerMock.Verify(
                l => l.LogError(It.IsAny<object>(), It.IsAny<string>()),
                Times.Never);
        }

        /// <summary>
        /// Tests that the Handle method returns a successful result with a null value when no text is found for the specified streetcode ID, but the streetcode itself exists in the repository.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ReturnsNullResult_WhenTextNotFoundButStreetcodeExists()
        {
            // Arrange
            var query = new GetTextByStreetcodeIdQuery(StreetcodeId: 1);

            this._textRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<TextEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TextEntity>, IIncludableQueryable<TextEntity, object>>?>()))
                .ReturnsAsync((TextEntity?)null);

            this._streetcodeRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                    It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>?>()))
                .ReturnsAsync(new StreetcodeContent { Id = 1 });

            // Act
            var result = await this._handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeNull();

            this._loggerMock.Verify(
                l => l.LogError(It.IsAny<object>(), It.IsAny<string>()),
                Times.Never);

            this._textServiceMock.Verify(
                s => s.AddTermsTag(It.IsAny<string>()),
                Times.Never);
        }

        /// <summary>
        /// Tests that the Handle method returns an error when no streetcode is found for the specified streetcode ID, regardless of whether a text exists or not.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ReturnsError_WhenStreetcodeDoesNotExist()
        {
            // Arrange
            var query = new GetTextByStreetcodeIdQuery(StreetcodeId: 1);

            this._textRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<TextEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TextEntity>, IIncludableQueryable<TextEntity, object>>?>()))
                .ReturnsAsync((TextEntity?)null);

            this._streetcodeRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                    It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>?>()))
                .ReturnsAsync((StreetcodeContent?)null);

            // Act
            var result = await this._handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsFailed.Should().BeTrue();

            this._loggerMock.Verify(
                l => l.LogError(query, It.Is<string>(msg => msg.Contains('1'))),
                Times.Once);

            this._textServiceMock.Verify(
                s => s.AddTermsTag(It.IsAny<string>()),
                Times.Never);
        }

        /// <summary>
        /// Tests that the Handle method processes the text content using the text service and updates the TextDTO with the processed content before returning it in the result when a text is found for the specified streetcode ID.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_UsesAddTermsTagResult_AsTextContent()
        {
            // Arrange
            const string originalContent = "Text about streetcode";
            const string taggedContent = "<Popover><Term>streetcode</Term><Desc>description</Desc></Popover>";
            var query = new GetTextByStreetcodeIdQuery(StreetcodeId: 1);
            var textEntity = new TextEntity
            {
                Id = 1,
                StreetcodeId = 1,
                TextContent = originalContent,
            };

            this._textRepositoryMock
                .Setup(repo => repo.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<TextEntity, bool>>>(),
                    It.IsAny<Func<IQueryable<TextEntity>, IIncludableQueryable<TextEntity, object>>?>()))
                .ReturnsAsync(textEntity);

            this._textServiceMock
                .Setup(s => s.AddTermsTag(originalContent))
                .ReturnsAsync(taggedContent);

            // Act
            var result = await this._handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value!.TextContent.Should().Be(taggedContent);
        }
    }
}