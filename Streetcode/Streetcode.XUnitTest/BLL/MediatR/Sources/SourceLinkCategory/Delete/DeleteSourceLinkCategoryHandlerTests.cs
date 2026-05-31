using System.Linq.Expressions;

using FluentAssertions;
using Moq;
using Xunit;

using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Sources.SourceLinkCategory.Delete;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Source;

using SourceLinkCategoryEntity = Streetcode.DAL.Entities.Sources.SourceLinkCategory;

namespace Streetcode.XUnitTest.BLL.MediatR.Sources.SourceLinkCategory.Delete;

public class DeleteSourceLinkCategoryHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<ISourceCategoryRepository> _sourceCategoryRepositoryMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly DeleteSourceLinkCategoryHandler _handler;

    public DeleteSourceLinkCategoryHandlerTests()
    {
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _sourceCategoryRepositoryMock = new Mock<ISourceCategoryRepository>();
        _loggerMock = new Mock<ILoggerService>();

        _repositoryWrapperMock
            .Setup(x => x.SourceCategoryRepository)
            .Returns(_sourceCategoryRepositoryMock.Object);

        _handler = new DeleteSourceLinkCategoryHandler(
            _repositoryWrapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenCategoryNotFound()
    {
        var command = new DeleteSourceLinkCategoryCommand(1);

        _sourceCategoryRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<SourceLinkCategoryEntity, bool>>>()))
            .ReturnsAsync((SourceLinkCategoryEntity?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.SourceCategoryNotFound);

        _sourceCategoryRepositoryMock.Verify(
            x => x.Delete(It.IsAny<SourceLinkCategoryEntity>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
    {
        var command = new DeleteSourceLinkCategoryCommand(1);

        var category = new SourceLinkCategoryEntity
        {
            Id = 1,
            Title = "News",
            ImageId = 5
        };

        _sourceCategoryRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<SourceLinkCategoryEntity, bool>>>()))
            .ReturnsAsync(category);

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.CannotDeleteSourceCategory);

        _sourceCategoryRepositoryMock.Verify(
            x => x.Delete(category),
            Times.Once);

        _repositoryWrapperMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCategoryDeletedSuccessfully()
    {
        var command = new DeleteSourceLinkCategoryCommand(1);

        var category = new SourceLinkCategoryEntity
        {
            Id = 1,
            Title = "News",
            ImageId = 5
        };

        _sourceCategoryRepositoryMock
            .Setup(x => x.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<SourceLinkCategoryEntity, bool>>>()))
            .ReturnsAsync(category);

        _repositoryWrapperMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(command.Id);

        _sourceCategoryRepositoryMock.Verify(
            x => x.Delete(category),
            Times.Once);

        _repositoryWrapperMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}