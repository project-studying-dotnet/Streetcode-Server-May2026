using System.Linq.Expressions;

using AutoMapper;
using FluentAssertions;
using Moq;
using Xunit;

using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Sources;
using Streetcode.BLL.MediatR.Sources.StreetcodeCategoryContent.Delete;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Source;

using StreetcodeCategoryContentEntity = Streetcode.DAL.Entities.Sources.StreetcodeCategoryContent;

namespace Streetcode.XUnitTest.BLL.MediatR.Sources.StreetcodeCategoryContent.Delete;

public class DeleteStreetcodeCategoryContentHandlerTests
{
    private const string ContentText = "Some content";
    private const int StreetcodeId = 1;
    private const int SourceLinkCategoryId = 2;

    private readonly IMapper _mapper;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IStreetcodeCategoryContentRepository> _streetcodeCategoryContentRepositoryMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly DeleteStreetcodeCategoryContentHandler _handler;

    public DeleteStreetcodeCategoryContentHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _streetcodeCategoryContentRepositoryMock = new Mock<IStreetcodeCategoryContentRepository>();
        _loggerMock = new Mock<ILoggerService>();

        _repositoryWrapperMock
            .Setup(x => x.StreetcodeCategoryContentRepository)
            .Returns(_streetcodeCategoryContentRepositoryMock.Object);

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<StreetcodeCategoryContentProfile>();
        });

        _mapper = mapperConfig.CreateMapper();

        _handler = new DeleteStreetcodeCategoryContentHandler(
            _repositoryWrapperMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenCategoryContentNotFound()
    {
        var command = new DeleteStreetcodeCategoryContentCommand(
            StreetcodeId: StreetcodeId,
            SourceLinkCategoryId: SourceLinkCategoryId);

        _streetcodeCategoryContentRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeCategoryContentEntity, bool>>>()))
            .ReturnsAsync((StreetcodeCategoryContentEntity?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors[0].Message.Should()
            .Be(ErrorMessages.SourceCategoryNotFound);

        _streetcodeCategoryContentRepositoryMock.Verify(
            x => x.Delete(It.IsAny<StreetcodeCategoryContentEntity>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
    {
        var command = new DeleteStreetcodeCategoryContentCommand(
            StreetcodeId: 1,
            SourceLinkCategoryId: 2);

        var content = new StreetcodeCategoryContentEntity
        {
            StreetcodeId = StreetcodeId,
            SourceLinkCategoryId = SourceLinkCategoryId,
            Text = ContentText
        };

        _streetcodeCategoryContentRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeCategoryContentEntity, bool>>>()))
            .ReturnsAsync(content);

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors[0].Message.Should()
            .Be(ErrorMessages.CannotDeleteSourceCategoryСontent);

        _streetcodeCategoryContentRepositoryMock.Verify(
            x => x.Delete(content),
            Times.Once);

        _repositoryWrapperMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCategoryContentDeletedSuccessfully()
    {
        var command = new DeleteStreetcodeCategoryContentCommand(
            StreetcodeId: 1,
            SourceLinkCategoryId: 2);

        var content = new StreetcodeCategoryContentEntity
        {
            StreetcodeId = StreetcodeId,
            SourceLinkCategoryId = SourceLinkCategoryId,
            Text = ContentText
        };

        _streetcodeCategoryContentRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeCategoryContentEntity, bool>>>()))
            .ReturnsAsync(content);

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        result.Value.Text.Should().Be(content.Text);
        result.Value.StreetcodeId.Should().Be(content.StreetcodeId);
        result.Value.SourceLinkCategoryId.Should().Be(content.SourceLinkCategoryId);

        _streetcodeCategoryContentRepositoryMock.Verify(
            x => x.Delete(content),
            Times.Once);

        _repositoryWrapperMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}