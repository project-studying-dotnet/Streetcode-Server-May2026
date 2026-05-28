using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Media.Images;
using Streetcode.BLL.Mapping.Newss;
using Streetcode.BLL.MediatR.Newss.SortedByDateTime;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using ImageEntity = global::Streetcode.DAL.Entities.Media.Images.Image;
using NewsEntity = global::Streetcode.DAL.Entities.News.News;

namespace Streetcode.XUnitTest.BLL.MediatR.News.SortedByDateTime;

public class SortedByDateTimeHandlerTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IBlobService> _blobServiceMock;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly SortedByDateTimeHandler _handler;

    public SortedByDateTimeHandlerTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<NewsProfile>();
            cfg.AddProfile<ImageProfile>();
        }).CreateMapper();

        _repositoryWrapperMock = new Mock<IRepositoryWrapper>
        {
            DefaultValue = DefaultValue.Mock,
        };

        _blobServiceMock = new Mock<IBlobService>();
        _loggerMock = new Mock<ILoggerService>();

        _handler = new SortedByDateTimeHandler(
            _repositoryWrapperMock.Object,
            _mapper,
            _blobServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenNewsIsNull()
    {
        var request = new SortedByDateTimeQuery();

        _repositoryWrapperMock
            .Setup(r => r.NewsRepository.GetAllAsync(
                It.IsAny<Expression<Func<NewsEntity, bool>>>(),
                It.IsAny<Func<
                    IQueryable<NewsEntity>,
                    IIncludableQueryable<NewsEntity, object>>>()))
            .ReturnsAsync((IEnumerable<NewsEntity>)null!);

        var result = await _handler.Handle(
            request,
            CancellationToken.None);

        Assert.True(result.IsFailed);

        Assert.Equal(
            ErrorMessages.ThereAreNoNewsInDatabase,
            result.Errors[0].Message);

        _loggerMock.Verify(
            l => l.LogError(
                request,
                ErrorMessages.ThereAreNoNewsInDatabase),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnOkAndSortDescending_WhenNewsExist()
    {
        var request = new SortedByDateTimeQuery();

        var newsEntities = new List<NewsEntity>
        {
            new()
            {
                Id = 1,
                CreationDate = new DateTime(2020, 1, 1),
                Image = null,
            },
            new()
            {
                Id = 2,
                CreationDate = new DateTime(2023, 1, 1),
                Image = new ImageEntity
                {
                    BlobName = "test.jpg",
                },
            },
        };

        const string expectedBase64 = "base64-encoded-string";

        _repositoryWrapperMock
            .Setup(r => r.NewsRepository.GetAllAsync(
                It.IsAny<Expression<Func<NewsEntity, bool>>>(),
                It.IsAny<Func<
                    IQueryable<NewsEntity>,
                    IIncludableQueryable<NewsEntity, object>>>()))
            .ReturnsAsync(newsEntities);

        _blobServiceMock
            .Setup(b => b.FindFileInStorageAsBase64("test.jpg"))
            .Returns(expectedBase64);

        var result = await _handler.Handle(
            request,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        Assert.Equal(2, result.Value.Count);

        Assert.Equal(2, result.Value[0].Id);
        Assert.Equal(1, result.Value[1].Id);

        Assert.NotNull(result.Value);
        Assert.NotNull(result.Value[0].Image);

        Assert.Equal(
            expectedBase64,
            result.Value[0].Image!.Base64);
    }
}