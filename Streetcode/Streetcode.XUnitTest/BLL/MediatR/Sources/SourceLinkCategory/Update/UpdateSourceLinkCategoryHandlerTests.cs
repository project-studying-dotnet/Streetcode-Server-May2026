using System.Linq.Expressions;

using AutoMapper;
using FluentAssertions;
using Moq;
using Xunit;

using Streetcode.BLL.DTO.Sources;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Sources;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Update;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Source;

using SourceLinkCategoryEntity = Streetcode.DAL.Entities.Sources.SourceLinkCategory;

namespace Streetcode.XUnitTest.BLL.MediatR.Sources.SourceLinkCategory.Update;

public class UpdateSourceLinkCategoryHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ISourceCategoryRepository> _sourceCategoryRepositoryMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly IMapper _mapper;
    private readonly UpdateSourceLinkCategoryHandler _handler;

    public UpdateSourceLinkCategoryHandlerTests()
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

        _handler = new UpdateSourceLinkCategoryHandler(
            _repositoryWrapperMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenIdIsInvalid()
    {
        var dto = new SourceLinkCategoryDTO
        {
            Id = 0,
            Title = "News",
            ImageId = 5
        };

        var command = new UpdateSourceLinkCategoryCommand(dto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.SourceCategoryIdRequired);

        _sourceCategoryRepositoryMock.Verify(x =>
            x.Update(It.IsAny<SourceLinkCategoryEntity>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenTitleIsEmpty()
    {
        var dto = new SourceLinkCategoryDTO
        {
            Id = 1,
            Title = "",
            ImageId = 5
        };

        var command = new UpdateSourceLinkCategoryCommand(dto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.SourceCategoryTitleRequired);

        _sourceCategoryRepositoryMock.Verify(x =>
            x.Update(It.IsAny<SourceLinkCategoryEntity>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenTitleIsTooLong()
    {
        var dto = new SourceLinkCategoryDTO
        {
            Id = 1,
            Title = new string('a', 24),
            ImageId = 5
        };

        var command = new UpdateSourceLinkCategoryCommand(dto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.SourceCategoryTitleTooLong);

        _sourceCategoryRepositoryMock.Verify(x =>
            x.Update(It.IsAny<SourceLinkCategoryEntity>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenImageIdIsInvalid()
    {
        var dto = new SourceLinkCategoryDTO
        {
            Id = 1,
            Title = "News",
            ImageId = 0
        };

        var command = new UpdateSourceLinkCategoryCommand(dto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.SourceCategoryImageRequired);

        _sourceCategoryRepositoryMock.Verify(x =>
            x.Update(It.IsAny<SourceLinkCategoryEntity>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenCategoryNotFound()
    {
        var dto = new SourceLinkCategoryDTO
        {
            Id = 1,
            Title = "News",
            ImageId = 5
        };

        var command = new UpdateSourceLinkCategoryCommand(dto);

        _sourceCategoryRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<SourceLinkCategoryEntity, bool>>>(),
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((SourceLinkCategoryEntity?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.SourceCategoryNotFound);

        _sourceCategoryRepositoryMock.Verify(x =>
            x.Update(It.IsAny<SourceLinkCategoryEntity>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenCategoryWithSameTitleExists()
    {
        var dto = new SourceLinkCategoryDTO
        {
            Id = 1,
            Title = "News",
            ImageId = 5
        };

        var category = new SourceLinkCategoryEntity
        {
            Id = 1,
            Title = "Old",
            ImageId = 4
        };

        var sameTitleCategory = new SourceLinkCategoryEntity
        {
            Id = 2,
            Title = "News",
            ImageId = 6
        };

        var command = new UpdateSourceLinkCategoryCommand(dto);

        _sourceCategoryRepositoryMock
            .SetupSequence(x => x.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<SourceLinkCategoryEntity, bool>>>(),
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(category)
            .ReturnsAsync(sameTitleCategory);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.SourceCategoryAlreadyExists);

        _sourceCategoryRepositoryMock.Verify(x =>
            x.Update(It.IsAny<SourceLinkCategoryEntity>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
    {
        var dto = new SourceLinkCategoryDTO
        {
            Id = 1,
            Title = "News",
            ImageId = 5
        };

        var category = new SourceLinkCategoryEntity
        {
            Id = 1,
            Title = "Old",
            ImageId = 4
        };

        var command = new UpdateSourceLinkCategoryCommand(dto);

        _sourceCategoryRepositoryMock
            .SetupSequence(x => x.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<SourceLinkCategoryEntity, bool>>>(),
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(category)
            .ReturnsAsync((SourceLinkCategoryEntity?)null);

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.CannotUpdateSourceCategory);

        _sourceCategoryRepositoryMock.Verify(x =>
            x.Update(It.IsAny<SourceLinkCategoryEntity>()),
            Times.Once);

        _repositoryWrapperMock.Verify(x =>
            x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCategoryUpdatedSuccessfully()
    {
        var dto = new SourceLinkCategoryDTO
        {
            Id = 1,
            Title = "News",
            ImageId = 5
        };

        var category = new SourceLinkCategoryEntity
        {
            Id = 1,
            Title = "Old",
            ImageId = 4
        };

        var command = new UpdateSourceLinkCategoryCommand(dto);

        _sourceCategoryRepositoryMock
            .SetupSequence(x => x.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<SourceLinkCategoryEntity, bool>>>(),
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(category)
            .ReturnsAsync((SourceLinkCategoryEntity?)null);

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(dto.Id);
        result.Value.Title.Should().Be(dto.Title);
        result.Value.ImageId.Should().Be(dto.ImageId);

        _sourceCategoryRepositoryMock.Verify(x =>
            x.Update(It.IsAny<SourceLinkCategoryEntity>()),
            Times.Once);

        _repositoryWrapperMock.Verify(x =>
            x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}