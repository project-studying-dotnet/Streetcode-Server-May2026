using AutoMapper;
using FluentAssertions;
using Moq;
using Xunit;

using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Sources;
using Streetcode.BLL.MediatR.Sources.StreetcodeCategoryContent.Create;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Source;

using StreetcodeCategoryContentEntity = Streetcode.DAL.Entities.Sources.StreetcodeCategoryContent;

namespace Streetcode.XUnitTest.BLL.MediatR.Sources.StreetcodeCategoryContent.Create;

public class CreateStreetcodeCategoryContentHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IStreetcodeCategoryContentRepository> _streetcodeCategoryContentRepositoryMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly CreateStreetcodeCategoryContentHandler _handler;

    public CreateStreetcodeCategoryContentHandlerTests()
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

        _handler = new CreateStreetcodeCategoryContentHandler(
            _repositoryWrapperMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCategoryContentCreatedSuccessfully()
    {
        var dto = new CategoryContentCreateDTO
        {
            Text = "Some content",
            StreetcodeId = 1,
            SourceLinkCategoryId = 2
        };

        var command = new CreateStreetcodeCategoryContentCommand(dto);

        _streetcodeCategoryContentRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<StreetcodeCategoryContentEntity>()))
            .ReturnsAsync((StreetcodeCategoryContentEntity content) => content);

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        result.Value.Text.Should().Be(dto.Text);
        result.Value.StreetcodeId.Should().Be(dto.StreetcodeId);
        result.Value.SourceLinkCategoryId.Should().Be(dto.SourceLinkCategoryId);

        _streetcodeCategoryContentRepositoryMock.Verify(
            x => x.CreateAsync(It.IsAny<StreetcodeCategoryContentEntity>()),
            Times.Once);

        _repositoryWrapperMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
    {
        var dto = new CategoryContentCreateDTO
        {
            Text = "Some content",
            StreetcodeId = 1,
            SourceLinkCategoryId = 2
        };

        var command = new CreateStreetcodeCategoryContentCommand(dto);

        _streetcodeCategoryContentRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<StreetcodeCategoryContentEntity>()))
            .ReturnsAsync(new StreetcodeCategoryContentEntity());

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();

        result.Errors[0].Message.Should()
            .Be(ErrorMessages.CannotSaveSourceCategoryContent);

        _streetcodeCategoryContentRepositoryMock.Verify(
            x => x.CreateAsync(It.IsAny<StreetcodeCategoryContentEntity>()),
            Times.Once);

        _repositoryWrapperMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}