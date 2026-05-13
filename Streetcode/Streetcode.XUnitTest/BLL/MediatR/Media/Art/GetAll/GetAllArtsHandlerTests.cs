using AutoMapper;
using FluentResults;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Repositories.Interfaces;
using Streetcode.BLL.DTO.Media.Art;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Media.Art.GetAll;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.Media.Art.GetAll;

/// <summary>
/// Tests for GetAllArtsHandler.
/// </summary>
public class GetAllArtsHandlerTests
{
    private GetAllArtsHandler handler;

    private Mock<IMapper> mockMapper;
    private Mock<IRepositoryWrapper> mockRepository;
    private Mock<IArtRepository> mockArtRepository;
    private Mock<ILoggerService> mockLoggerService;

    private IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllArtsHandlerTests"/> class.
    /// </summary>
    public GetAllArtsHandlerTests()
    {
        this.mockMapper = new Mock<IMapper>();
        this.mockRepository = new Mock<IRepositoryWrapper>();
        this.mockArtRepository = new Mock<IArtRepository>();
        this.mockLoggerService = new Mock<ILoggerService>();

        this.mockRepository
                .Setup(r => r.ArtRepository)
                .Returns(this.mockArtRepository.Object);

        this.mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<DAL.Entities.Media.Images.Art, ArtDTO>();
        }).CreateMapper();

        this.handler = new GetAllArtsHandler(
                this.mockRepository.Object,
                this.mapper,
                this.mockLoggerService.Object);
    }

    /// <summary>
    /// Method Handel returns all Arts, if they are.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task Handle_ArtsNotEmpty_ReturnAllArts()
    {
        var query = new GetAllArtsQuery();

        int expectedCount = 1;

        var arts = new List<DAL.Entities.Media.Images.Art>()
        {
            new DAL.Entities.Media.Images.Art()
            {
                Id = 1,
                Description = "Description art 1",
                ImageId = 1,
                Title = "Title art 1",
            },
        };

        this.mockArtRepository
                    .Setup(r => r.GetAllAsync(
                        It.IsAny<Expression<Func<DAL.Entities.Media.Images.Art, bool>>>(),
                        It.IsAny<Func<IQueryable<DAL.Entities.Media.Images.Art>,
                            IIncludableQueryable<DAL.Entities.Media.Images.Art, object>>>()))
                    .ReturnsAsync(arts);

        var result = await this.handler.Handle(query, CancellationToken.None);

        Assert.Equal(expectedCount, result.Value.Count());
    }

    /// <summary>
    /// Method Handle return error message, if Arts do not exist.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task Handle_ArtsIsEmpty_ReturnErrorMessage()
    {
        var query = new GetAllArtsQuery();

        var expectedError = new Error($"Cannot find any arts");

        this.mockArtRepository
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<DAL.Entities.Media.Images.Art, bool>>>(),
                    It.IsAny<Func<IQueryable<DAL.Entities.Media.Images.Art>,
                        IIncludableQueryable<DAL.Entities.Media.Images.Art, object>>>()))
                .ReturnsAsync((List<DAL.Entities.Media.Images.Art>?)null);

        var result = await this.handler.Handle(query, CancellationToken.None);

        Assert.Equal(expectedError.Message, result.Errors.First().Message);
    }
}
