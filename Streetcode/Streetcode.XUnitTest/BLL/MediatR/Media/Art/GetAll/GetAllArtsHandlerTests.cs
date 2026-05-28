using System.Linq.Expressions;
using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Repositories.Interfaces;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Art.GetAll;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using ArtEntity = Streetcode.DAL.Entities.Media.Images.Art;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Art.GetAll;

public class GetAllArtsHandlerTests
{
    private readonly GetAllArtsHandler _handler;
    private readonly Mock<IRepositoryWrapper> _mockRepository;
    private readonly Mock<IArtRepository> _mockArtRepository;
    private readonly Mock<ILoggerService> _mockLoggerService;

    public GetAllArtsHandlerTests()
    {
        _mockRepository = new Mock<IRepositoryWrapper>();
        _mockArtRepository = new Mock<IArtRepository>();
        _mockLoggerService = new Mock<ILoggerService>();

        _mockRepository
            .Setup(r => r.ArtRepository)
            .Returns(_mockArtRepository.Object);

        var mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ArtEntity, ArtDTO>();
        }).CreateMapper();

        _handler = new GetAllArtsHandler(
            _mockRepository.Object,
            mapper,
            _mockLoggerService.Object);
    }

    [Fact]
    public async Task Handle_ArtsNotEmpty_ReturnAllArts()
    {
        var query = new GetAllArtsQuery();

        const string expectedTitle = "Title art 1";

        var arts = new List<ArtEntity>
        {
            new()
            {
                Id = 1,
                Description = "Description art 1",
                ImageId = 1,
                Title = expectedTitle,
            },
        };

        _mockArtRepository
            .Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<ArtEntity, bool>>>(),
                It.IsAny<Func<IQueryable<ArtEntity>, IIncludableQueryable<ArtEntity, object>>>()))
            .ReturnsAsync(arts);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        var art = Assert.Single(result.Value);
        Assert.Equal(expectedTitle, art.Title);
        Assert.IsType<ArtDTO>(art);

        _mockArtRepository.Verify(
            r => r.GetAllAsync(
                It.IsAny<Expression<Func<ArtEntity, bool>>>(),
                It.IsAny<Func<IQueryable<ArtEntity>, IIncludableQueryable<ArtEntity, object>>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ArtsIsEmpty_ReturnErrorMessage()
    {
        var query = new GetAllArtsQuery();

        var expectedError = new Error(ErrorMessages.CannotFindAnyArts);

        _mockArtRepository
            .Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<ArtEntity, bool>>>(),
                It.IsAny<Func<IQueryable<ArtEntity>, IIncludableQueryable<ArtEntity, object>>>()))
            .ReturnsAsync((List<ArtEntity>)null!);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Equal(expectedError.Message, result.Errors.First().Message);

        _mockArtRepository.Verify(
            r => r.GetAllAsync(
                It.IsAny<Expression<Func<ArtEntity, bool>>>(),
                It.IsAny<Func<IQueryable<ArtEntity>, IIncludableQueryable<ArtEntity, object>>>()),
            Times.Once);
    }
}