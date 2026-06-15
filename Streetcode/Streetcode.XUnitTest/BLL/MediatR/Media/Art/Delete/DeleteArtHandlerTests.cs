using System.Linq.Expressions;
using MediatR;
using Moq;
using Repositories.Interfaces;
using Streetcode.BLL.MediatR.Media.Art.Delete;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using ArtEntity = Streetcode.DAL.Entities.Media.Images.Art;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Art.Delete;

public class DeleteArtHandlerTests
{
    private readonly DeleteArtHandler _handler;
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IArtRepository> _mockArtRepository;

    public DeleteArtHandlerTests()
    {
        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockArtRepository = new Mock<IArtRepository>();

        _mockRepository
            .Setup(r => r.ArtRepository)
            .Returns(_mockArtRepository.Object);

        _handler = new DeleteArtHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ArtExists_DeleteArt()
    {
        // Arrange
        const int artId = 1;

        var art = new ArtEntity
        {
            Id = artId,
            Title = "Test Art",
        };

        _mockArtRepository
            .Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<ArtEntity, bool>>>(),
                null))
            .ReturnsAsync(art);

        var command = new DeleteArtCommand(artId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(Unit.Value, result.Value);

        _mockArtRepository.Verify(
            r => r.Delete(art),
            Times.Once);

        _mockRepository.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ArtNotFound_ReturnError()
    {
        // Arrange
        const int artId = 1;

        _mockArtRepository
            .Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<ArtEntity, bool>>>(),
                null))
            .ReturnsAsync((ArtEntity?)null);

        var command = new DeleteArtCommand(artId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            string.Format(ErrorMessages.EntityNotFound, artId),
            result.Errors.First().Message);

        _mockArtRepository.Verify(
            r => r.Delete(It.IsAny<ArtEntity>()),
            Times.Never);

        _mockRepository.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}