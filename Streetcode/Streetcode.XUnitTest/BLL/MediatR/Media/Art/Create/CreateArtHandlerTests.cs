using AutoMapper;
using Moq;
using Repositories.Interfaces;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.MediatR.Media.Art.Create;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using ArtEntity = Streetcode.DAL.Entities.Media.Images.Art;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Art.Create;

public class CreateArtHandlerTests
{
    private readonly CreateArtHandler _handler;
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IArtRepository> _mockArtRepository;

    public CreateArtHandlerTests()
    {
        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockArtRepository = new Mock<IArtRepository>();

        _mockRepository
            .Setup(r => r.ArtRepository)
            .Returns(_mockArtRepository.Object);

        var mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ArtCreateDto, ArtEntity>();
            cfg.CreateMap<ArtEntity, ArtDTO>();
        }).CreateMapper();

        _handler = new CreateArtHandler(
            mapper,
            _mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnCreatedArt()
    {
        // Arrange
        const string expectedTitle = "Test Art";

        var artDto = new ArtCreateDto
        {
            Title = expectedTitle,
        };

        var command = new CreateArtCommand(artDto);

        _mockArtRepository
            .Setup(r => r.CreateAsync(It.IsAny<ArtEntity>()))
            .ReturnsAsync(new ArtEntity());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(expectedTitle, result.Value.Title);
        Assert.IsType<ArtDTO>(result.Value);

        _mockArtRepository.Verify(
            r => r.CreateAsync(It.IsAny<ArtEntity>()),
            Times.Once);

        _mockRepository.Verify(
            r => r.SaveChangesAsync(),
            Times.Once);
    }
}