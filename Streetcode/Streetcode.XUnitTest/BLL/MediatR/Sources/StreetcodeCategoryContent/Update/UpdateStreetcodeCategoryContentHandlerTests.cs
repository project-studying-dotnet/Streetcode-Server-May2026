using System.Linq.Expressions;

using AutoMapper;
using FluentAssertions;
using Moq;
using Xunit;

using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Sources;
using Streetcode.BLL.MediatR.Sources.StreetcodeCategoryContent.Update;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Source;

using StreetcodeCategoryContentEntity = Streetcode.DAL.Entities.Sources.StreetcodeCategoryContent;

namespace Streetcode.XUnitTest.BLL.MediatR.Sources.StreetcodeCategoryContent.Update;

public class UpdateStreetcodeCategoryContentHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IStreetcodeCategoryContentRepository> _streetcodeCategoryContentRepositoryMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly UpdateStreetcodeCategoryContentHandler _handler;

    public UpdateStreetcodeCategoryContentHandlerTests()
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

        _handler = new UpdateStreetcodeCategoryContentHandler(
            _repositoryWrapperMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenCategoryContentNotFound()
    {
        var dto = new CategoryContentUpdateDTO
        {
            Text = "Updated content",
            StreetcodeId = 1,
            SourceLinkCategoryId = 2
        };

        var command = new UpdateStreetcodeCategoryContentCommand(dto);

        _streetcodeCategoryContentRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeCategoryContentEntity, bool>>>()))
            .ReturnsAsync((StreetcodeCategoryContentEntity?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors[0].Message.Should()
            .Be(ErrorMessages.SourceCategoryNotFound);

        _streetcodeCategoryContentRepositoryMock.Verify(
            x => x.Update(It.IsAny<StreetcodeCategoryContentEntity>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
    {
        var dto = new CategoryContentUpdateDTO
        {
            Text = "Updated content",
            StreetcodeId = 1,
            SourceLinkCategoryId = 2
        };

        var existingContent = new StreetcodeCategoryContentEntity
        {
            Text = "Old content",
            StreetcodeId = 1,
            SourceLinkCategoryId = 2
        };

        var command = new UpdateStreetcodeCategoryContentCommand(dto);

        _streetcodeCategoryContentRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeCategoryContentEntity, bool>>>()))
            .ReturnsAsync(existingContent);

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors[0].Message.Should()
            .Be(ErrorMessages.CannotUpdateStreetcodeCategoryContent);

        _streetcodeCategoryContentRepositoryMock.Verify(
            x => x.Update(It.Is<StreetcodeCategoryContentEntity>(
                content => content.Text == dto.Text)),
            Times.Once);

        _repositoryWrapperMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCategoryContentUpdatedSuccessfully()
    {
        var dto = new CategoryContentUpdateDTO
        {
            Text = "Updated content",
            StreetcodeId = 1,
            SourceLinkCategoryId = 2
        };

        var existingContent = new StreetcodeCategoryContentEntity
        {
            Text = "Old content",
            StreetcodeId = 1,
            SourceLinkCategoryId = 2
        };

        var command = new UpdateStreetcodeCategoryContentCommand(dto);

        _streetcodeCategoryContentRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeCategoryContentEntity, bool>>>()))
            .ReturnsAsync(existingContent);

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        result.Value.Text.Should().Be(dto.Text);
        result.Value.StreetcodeId.Should().Be(dto.StreetcodeId);
        result.Value.SourceLinkCategoryId.Should().Be(dto.SourceLinkCategoryId);

        _streetcodeCategoryContentRepositoryMock.Verify(
            x => x.Update(It.Is<StreetcodeCategoryContentEntity>(
                content =>
                    content.Text == dto.Text &&
                    content.StreetcodeId == dto.StreetcodeId &&
                    content.SourceLinkCategoryId == dto.SourceLinkCategoryId)),
            Times.Once);

        _repositoryWrapperMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}