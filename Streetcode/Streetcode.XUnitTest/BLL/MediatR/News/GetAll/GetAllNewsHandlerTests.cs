using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Media.Images;
using Streetcode.BLL.Mapping.Newss;
using Streetcode.BLL.MediatR.Newss.GetAll;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;
using Streetcode.BLL.Resources;

namespace Streetcode.XUnitTest.BLL.MediatR.News.GetAll
{
    public class GetAllNewsHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IBlobService> _blobServiceMock;
        private readonly Mock<ILoggerService> _loggerMock;

        private readonly GetAllNewsHandler _handler;

        public GetAllNewsHandlerTests()
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NewsProfile>();
                cfg.AddProfile<ImageProfile>();
            }).CreateMapper();

            _repositoryWrapperMock = new Mock<IRepositoryWrapper> { DefaultValue = DefaultValue.Mock };
            _blobServiceMock = new Mock<IBlobService>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new GetAllNewsHandler(
                _repositoryWrapperMock.Object,
                _mapper,
                _blobServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenNewsIsNull()
        {
            var request = new GetAllNewsQuery();

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetAllAsync(
                It.IsAny<Expression<Func<DAL.Entities.News.News, bool>>>(),
                It.IsAny<Func<IQueryable<DAL.Entities.News.News>, IIncludableQueryable<DAL.Entities.News.News, object>>>()))
                .ReturnsAsync((IEnumerable<DAL.Entities.News.News>)null);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsFailed);
            Assert.Equal(ErrorMessages.ThereAreNoNewsInDatabase, result.Errors[0].Message);
            _loggerMock.Verify(l => l.LogError(request, ErrorMessages.ThereAreNoNewsInDatabase), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnOk_WhenNewsHasNoImages()
        {
            var request = new GetAllNewsQuery();

            var newsEntities = new List<DAL.Entities.News.News>
            {
                new DAL.Entities.News.News { Id = 1, Image = null }
            };

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetAllAsync(
                It.IsAny<Expression<Func<DAL.Entities.News.News, bool>>>(),
                It.IsAny<Func<IQueryable<DAL.Entities.News.News>, IIncludableQueryable<DAL.Entities.News.News, object>>>()))
                .ReturnsAsync(newsEntities);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);

            Assert.Single(result.Value);
            Assert.Equal(1, result.Value.First().Id);
            Assert.Null(result.Value.First().Image);

            _blobServiceMock.Verify(b => b.FindFileInStorageAsBase64(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnOkAndFetchBase64_WhenNewsHasImages()
        {
            var request = new GetAllNewsQuery();

            var newsEntities = new List<DAL.Entities.News.News>
            {
                new DAL.Entities.News.News
                {
                    Id = 1,
                    Image = new DAL.Entities.Media.Images.Image { BlobName = "test-image.jpg" },
                },
            };

            var expectedBase64 = "base64-encoded-string";

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetAllAsync(
                It.IsAny<Expression<Func<DAL.Entities.News.News, bool>>>(),
                It.IsAny<Func<IQueryable<DAL.Entities.News.News>, IIncludableQueryable<DAL.Entities.News.News, object>>>()))
                .ReturnsAsync(newsEntities);

            _blobServiceMock.Setup(b => b.FindFileInStorageAsBase64("test-image.jpg"))
                .Returns(expectedBase64);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Single(result.Value);
            Assert.Equal(expectedBase64, result.Value.First().Image.Base64);

            _blobServiceMock.Verify(b => b.FindFileInStorageAsBase64("test-image.jpg"), Times.Once);
        }
    }
}
