using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Sources;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Create;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Source;
using System.Linq.Expressions;
using Xunit;
using SourceLinkCategoryEntity = Streetcode.DAL.Entities.Sources.SourceLinkCategory;

namespace Streetcode.XUnitTest.BLL.MediatR.Sources.SourceLinkCategory.Create;

public class CreateSourceLinkCategoryHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ISourceCategoryRepository> _sourceCategoryRepositoryMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly IMapper _mapper;
    private readonly CreateSourceLinkCategoryHandler _handler;

    public CreateSourceLinkCategoryHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _sourceCategoryRepositoryMock = new Mock<ISourceCategoryRepository>();
        _loggerMock = new Mock<ILoggerService>();

        _repositoryWrapperMock
            .Setup(x => x.SourceCategoryRepository)
            .Returns(_sourceCategoryRepositoryMock.Object);

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SourceLinkCategoryProfile>();
        });

        _mapper = mapperConfig.CreateMapper();

        _handler = new CreateSourceLinkCategoryHandler(
            _repositoryWrapperMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCategoryCreatedSuccessfully()
    {
        var dto = new SourceLinkCategoryDTO
        {
            Title = "News",
            ImageId = 5
        };

        var command = new CreateSourceLinkCategoryCommand(dto);

        _sourceCategoryRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<SourceLinkCategoryEntity, bool>>>()))
            .ReturnsAsync((SourceLinkCategoryEntity?)null);

        _sourceCategoryRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<SourceLinkCategoryEntity>()))
            .ReturnsAsync((SourceLinkCategoryEntity category) =>
            {
                category.Id = 1;
                return category;
            });

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(1);
        result.Value.Title.Should().Be(dto.Title);
        result.Value.ImageId.Should().Be(dto.ImageId);

        _sourceCategoryRepositoryMock.Verify(x =>
            x.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<SourceLinkCategoryEntity, bool>>>()),
            Times.Once);

        _sourceCategoryRepositoryMock.Verify(x =>
            x.CreateAsync(It.IsAny<SourceLinkCategoryEntity>()),
            Times.Once);

        _repositoryWrapperMock.Verify(x =>
            x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenCategoryAlreadyExists()
    {
        var dto = new SourceLinkCategoryDTO
        {
            Title = "News",
            ImageId = 5
        };

        var existingCategory = new SourceLinkCategoryEntity
        {
            Id = 1,
            Title = "News",
            ImageId = 5
        };

        var command = new CreateSourceLinkCategoryCommand(dto);

        _sourceCategoryRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<SourceLinkCategoryEntity, bool>>>()))
            .ReturnsAsync(existingCategory);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .Be(ErrorMessages.SourceCategoryAlreadyExists);

        _sourceCategoryRepositoryMock.Verify(x =>
            x.CreateAsync(It.IsAny<SourceLinkCategoryEntity>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
    {
        var dto = new SourceLinkCategoryDTO
        {
            Title = "News",
            ImageId = 5
        };

        var command = new CreateSourceLinkCategoryCommand(dto);

        _sourceCategoryRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<SourceLinkCategoryEntity, bool>>>()))
            .ReturnsAsync((SourceLinkCategoryEntity?)null);

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .Be(ErrorMessages.CannotSaveSourceCategory);

        _sourceCategoryRepositoryMock.Verify(x =>
            x.CreateAsync(It.IsAny<SourceLinkCategoryEntity>()),
            Times.Once);

        _repositoryWrapperMock.Verify(x =>
            x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}