using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Newss.GetByUrl;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.News.GetByUrl
{
    public class GetNewsByUrlHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IBlobService> _blobServiceMock;
        private readonly Mock<ILoggerService> _loggerMock;

        private readonly GetNewsByUrlHandler _handler;

        public GetNewsByUrlHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _repositoryWrapperMock = new Mock<IRepositoryWrapper> { DefaultValue = DefaultValue.Mock };
            _blobServiceMock = new Mock<IBlobService>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new GetNewsByUrlHandler(
                _mapperMock.Object,
                _repositoryWrapperMock.Object,
                _blobServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenNewsNotFound()
        {
            string testUrl = "example-news-url";
            var request = new GetNewsByUrlQuery(testUrl);
            var errorMsg = $"No news by entered Url - {testUrl}";

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<DAL.Entities.News.News, bool>>>(),
                It.IsAny<Func<IQueryable<DAL.Entities.News.News>, IIncludableQueryable<DAL.Entities.News.News, object>>>()))
                .ReturnsAsync((DAL.Entities.News.News)null);

            _mapperMock.Setup(m => m.Map<NewsDTO>(It.IsAny<DAL.Entities.News.News>()))
                .Returns((NewsDTO)null);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsFailed);
            Assert.Equal(errorMsg, result.Errors[0].Message);
            _loggerMock.Verify(l => l.LogError(request, errorMsg), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnOk_WhenNewsHasNoImage()
        {
            string testUrl = "example-news-url";
            var request = new GetNewsByUrlQuery(testUrl);
            var newsEntity = new DAL.Entities.News.News { URL = testUrl };
            var newsDTO = new NewsDTO { URL = testUrl, Image = null };

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<DAL.Entities.News.News, bool>>>(),
                It.IsAny<Func<IQueryable<DAL.Entities.News.News>, IIncludableQueryable<DAL.Entities.News.News, object>>>()))
                .ReturnsAsync(newsEntity);

            _mapperMock.Setup(m => m.Map<NewsDTO>(newsEntity))
                .Returns(newsDTO);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(newsDTO, result.Value);
            _blobServiceMock.Verify(b => b.FindFileInStorageAsBase64(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnOkAndFetchBase64_WhenNewsHasImage()
        {
            string testUrl = "example-news-url";
            var request = new GetNewsByUrlQuery(testUrl);
            var newsEntity = new DAL.Entities.News.News { URL = testUrl };
            var newsDTO = new NewsDTO
            {
                URL = testUrl,
                Image = new ImageDTO { BlobName = "test-image.jpg" }
            };
            var expectedBase64 = "base64-encoded-string";

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<DAL.Entities.News.News, bool>>>(),
                It.IsAny<Func<IQueryable<DAL.Entities.News.News>, IIncludableQueryable<DAL.Entities.News.News, object>>>()))
                .ReturnsAsync(newsEntity);

            _mapperMock.Setup(m => m.Map<NewsDTO>(newsEntity))
                .Returns(newsDTO);

            _blobServiceMock.Setup(b => b.FindFileInStorageAsBase64("test-image.jpg"))
                .Returns(expectedBase64);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(expectedBase64, result.Value.Image.Base64);
            _blobServiceMock.Verify(b => b.FindFileInStorageAsBase64("test-image.jpg"), Times.Once);
        }
    }
}
