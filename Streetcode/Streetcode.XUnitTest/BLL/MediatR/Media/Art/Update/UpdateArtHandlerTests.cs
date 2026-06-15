using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Repositories.Interfaces;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.MediatR.Media.Art.Update;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using ArtEntity = Streetcode.DAL.Entities.Media.Images.Art;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Art.Update;

public class UpdateArtHandlerTests
{
    private readonly UpdateArtHandler _handler;
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IArtRepository> _mockArtRepository;

    public UpdateArtHandlerTests()
    {
        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockArtRepository = new Mock<IArtRepository>();

        _mockRepository
            .Setup(r => r.ArtRepository)
            .Returns(_mockArtRepository.Object);

        var mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ArtUpdateDto, ArtEntity>();
            cfg.CreateMap<ArtEntity, ArtDTO>();
        }).CreateMapper();

        _handler = new UpdateArtHandler(
            mapper,
            _mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ArtExists_ReturnUpdatedArt()
    {
        // Arrange
        const int artId = 1;
        const string updatedTitle = "Updated Art";

        var art = new ArtEntity
        {
            Id = artId,
            Title = "Old Title",
            Description = "Old Description",
            ImageId = 1,
        };

        var updateDto = new ArtUpdateDto
        {
            Id = artId,
            Title = updatedTitle,
            Description = "Updated Description",
            ImageId = 1,
        };

        _mockArtRepository
            .Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<ArtEntity, bool>>>(),
                It.IsAny<Func<IQueryable<ArtEntity>, IIncludableQueryable<ArtEntity, object>>>()))
            .ReturnsAsync(art);

        var command = new UpdateArtCommand(updateDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(updatedTitle, result.Value.Title);
        Assert.IsType<ArtDTO>(result.Value);

        _mockArtRepository.Verify(
            r => r.Update(It.IsAny<ArtEntity>()),
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

        var updateDto = new ArtUpdateDto
        {
            Id = artId,
            Title = "Updated Art",
            ImageId = 1,
        };

        _mockArtRepository
            .Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<ArtEntity, bool>>>(),
                It.IsAny<Func<IQueryable<ArtEntity>, IIncludableQueryable<ArtEntity, object>>>()))
            .ReturnsAsync((ArtEntity)null!);

        var command = new UpdateArtCommand(updateDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        Assert.Equal(
            string.Format(ErrorMessages.EntityNotFound, artId),
            result.Errors.First().Message);

        _mockArtRepository.Verify(
            r => r.Update(It.IsAny<ArtEntity>()),
            Times.Never);

        _mockRepository.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}