using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Repositories.Interfaces;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Art.GetById;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using ArtEntity = Streetcode.DAL.Entities.Media.Images.Art;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Art.GetById;

public class GetArtByIdHandlerTests
{
    private readonly GetArtByIdHandler _handler;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IArtRepository> _mockArtRepository;
    private readonly Mock<ILoggerService> _mockLoggerService;

    public GetArtByIdHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockArtRepository = new Mock<IArtRepository>();
        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockLoggerService = new Mock<ILoggerService>();

        _mockRepository
            .Setup(r => r.ArtRepository)
            .Returns(_mockArtRepository.Object);

        _handler = new GetArtByIdHandler(
            _mockRepository.Object,
            _mockMapper.Object,
            _mockLoggerService.Object);
    }

    [Fact]
    public async Task Handle_ValidId_ReturnArt()
    {
        var query = new GetArtByIdQuery(1);

        var art = new ArtEntity
        {
            Id = 1,
            Description = "Description art 1",
            ImageId = 1,
            Title = "Title art 1",
        };

        var artDto = new ArtDTO
        {
            Id = 1,
            Description = "Description art 1",
            ImageId = 1,
            Title = "Title art 1",
        };

        _mockArtRepository
            .Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<ArtEntity, bool>>>(),
                It.IsAny<Func<IQueryable<ArtEntity>, IIncludableQueryable<ArtEntity, object>>>()))
            .ReturnsAsync(art);

        _mockMapper
            .Setup(m => m.Map<ArtDTO>(It.IsAny<ArtEntity>()))
            .Returns(artDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(artDto.Id, result.Value.Id);
        Assert.Equal(artDto.Title, result.Value.Title);

        _mockArtRepository.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<ArtEntity, bool>>>(),
                It.IsAny<Func<IQueryable<ArtEntity>, IIncludableQueryable<ArtEntity, object>>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_NotValidId_ReturnError()
    {
        var query = new GetArtByIdQuery(2);
        var expectedErrorMsg = string.Format(ErrorMessages.CannotFindArtById, query.Id);

        _mockArtRepository
            .Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<ArtEntity, bool>>>(),
                It.IsAny<Func<IQueryable<ArtEntity>, IIncludableQueryable<ArtEntity, object>>>()))
            .ReturnsAsync((ArtEntity)null!);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(expectedErrorMsg, result.Errors.First().Message);

        _mockArtRepository.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<ArtEntity, bool>>>(),
                It.IsAny<Func<IQueryable<ArtEntity>, IIncludableQueryable<ArtEntity, object>>>()),
            Times.Once);
    }
}
