using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Text;
using Streetcode.BLL.Mapping.Streetcode.TextContent;
using Streetcode.BLL.MediatR.Streetcode.Text.GetByStreetcodeId;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Streetcode;
using Streetcode.DAL.Repositories.Interfaces.Streetcode.TextContent;
using Xunit;

using TextEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Text;

namespace Streetcode.XUnitTest.BLL.MediatR.StreetCode.Text;

public class GetTextByStreetcodeIdHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ITextRepository> _textRepositoryMock;
    private readonly Mock<IStreetcodeRepository> _streetcodeRepositoryMock;
    private readonly IMapper _mapper;
    private readonly Mock<ITextService> _textServiceMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetTextByStreetcodeIdHandler _handler;

    public GetTextByStreetcodeIdHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _textRepositoryMock = new Mock<ITextRepository>();
        _streetcodeRepositoryMock = new Mock<IStreetcodeRepository>();
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<TextProfile>()).CreateMapper();
        _textServiceMock = new Mock<ITextService>();
        _loggerMock = new Mock<ILoggerService>();

        _repositoryWrapperMock
            .Setup(w => w.TextRepository)
            .Returns(_textRepositoryMock.Object);

        _repositoryWrapperMock
            .Setup(w => w.StreetcodeRepository)
            .Returns(_streetcodeRepositoryMock.Object);

        _handler = new GetTextByStreetcodeIdHandler(
            _repositoryWrapperMock.Object,
            _mapper,
            _textServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsProcessedText_WhenTextExists()
    {
        const string originalContent = "Text about Maidan";
        const string taggedContent = "<Popover><Term>Maidan</Term><Desc>Central square</Desc></Popover>";

        var query = new GetTextByStreetcodeIdQuery(StreetcodeId: 1);

        var textEntity = new TextEntity
        {
            Id = 1,
            StreetcodeId = 1,
            TextContent = originalContent,
        };

        _textRepositoryMock
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TextEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TextEntity>,
                    IIncludableQueryable<TextEntity, object>>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(textEntity);

        _textServiceMock
            .Setup(service => service.AddTermsTag(originalContent))
            .ReturnsAsync(taggedContent);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        var value = result.Value!;

        value.TextContent.Should().Be(taggedContent);

        _textServiceMock.Verify(
            service => service.AddTermsTag(originalContent),
            Times.Once);

        _loggerMock.Verify(
            logger => logger.LogError(It.IsAny<object>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ReturnsNullResult_WhenTextNotFoundButStreetcodeExists()
    {
        var query = new GetTextByStreetcodeIdQuery(StreetcodeId: 1);

        _textRepositoryMock
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TextEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TextEntity>,
                    IIncludableQueryable<TextEntity, object>>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((TextEntity)null!);

        _streetcodeRepositoryMock
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeContent>,
                    IIncludableQueryable<StreetcodeContent, object>>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StreetcodeContent { Id = 1 });

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();

        _loggerMock.Verify(
            logger => logger.LogError(It.IsAny<object>(), It.IsAny<string>()),
            Times.Never);

        _textServiceMock.Verify(
            service => service.AddTermsTag(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenStreetcodeDoesNotExist()
    {
        var query = new GetTextByStreetcodeIdQuery(StreetcodeId: 1);

        _textRepositoryMock
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TextEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TextEntity>,
                    IIncludableQueryable<TextEntity, object>>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((TextEntity)null!);

        _streetcodeRepositoryMock
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeContent>,
                    IIncludableQueryable<StreetcodeContent, object>>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((StreetcodeContent)null!);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        _loggerMock.Verify(
            logger => logger.LogError(
                query,
                It.Is<string>(message => message.Contains('1'))),
            Times.Once);

        _textServiceMock.Verify(
            service => service.AddTermsTag(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_UsesAddTermsTagResult_AsTextContent()
    {
        const string originalContent = "Text about streetcode";
        const string taggedContent = "<Popover><Term>streetcode</Term><Desc>description</Desc></Popover>";

        var query = new GetTextByStreetcodeIdQuery(StreetcodeId: 1);

        var textEntity = new TextEntity
        {
            Id = 1,
            StreetcodeId = 1,
            TextContent = originalContent,
        };

        _textRepositoryMock
            .Setup(repo => repo.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TextEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TextEntity>,
                    IIncludableQueryable<TextEntity, object>>?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(textEntity);

        _textServiceMock
            .Setup(service => service.AddTermsTag(originalContent))
            .ReturnsAsync(taggedContent);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        var value = result.Value!;

        value.TextContent.Should().Be(taggedContent);
    }
}
