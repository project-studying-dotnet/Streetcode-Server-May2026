using System.Linq.Expressions;
using System.Transactions;
using FluentAssertions;
using Moq;
using Streetcode.BLL.MediatR.Media.ArtSlide.Delete;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Media.Images;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.ArtSlide.Delete;

public class DeleteArtSlideHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _repoWrapperMock;
    private readonly Mock<IStreetcodeArtSlideRepository> _slideRepoMock;
    private readonly Mock<IArtSlideItemRepository> _slideItemRepoMock;
    private readonly DeleteArtSlideHandler _handler;

    public DeleteArtSlideHandlerTests()
    {
        _repoWrapperMock = new Mock<IRepositoryWrapper>();
        _slideRepoMock = new Mock<IStreetcodeArtSlideRepository>();
        _slideItemRepoMock = new Mock<IArtSlideItemRepository>();

        _repoWrapperMock.Setup(r => r.StreetcodeArtSlideRepository).Returns(_slideRepoMock.Object);
        _repoWrapperMock.Setup(r => r.ArtSlideItemRepository).Returns(_slideItemRepoMock.Object);

        _handler = new DeleteArtSlideHandler(_repoWrapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeleteSlideAndItems_WhenSlideExists()
    {
        // Arrange
        int slideId = 1;
        var command = new DeleteArtSlideCommand(slideId);
        var slide = new StreetcodeArtSlide { Id = slideId };

        _slideRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeArtSlide, bool>>>(), null))
            .ReturnsAsync(slide);

        _repoWrapperMock.Setup(r => r.BeginTransaction())
            .Returns(new TransactionScope(TransactionScopeAsyncFlowOption.Enabled));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _slideItemRepoMock.Verify(r => r.DeleteRange(It.IsAny<IEnumerable<ArtSlideItem>>()), Times.Once);
        _slideRepoMock.Verify(r => r.Delete(It.IsAny<StreetcodeArtSlide>()), Times.Once);
        _repoWrapperMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSlideDoesNotExist()
    {
        // Arrange
        int slideId = 99;
        var command = new DeleteArtSlideCommand(slideId);

        _slideRepoMock.Setup(r => r.GetFirstOrDefaultAsync(It.IsAny<Expression<Func<StreetcodeArtSlide, bool>>>(), null))
            .ReturnsAsync((StreetcodeArtSlide)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains(string.Format(ErrorMessages.SlideNotFound, slideId)));
    }
}