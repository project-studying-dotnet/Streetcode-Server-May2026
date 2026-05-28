using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Newss;
using Streetcode.BLL.MediatR.Newss.GetByUrl;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using Streetcode.BLL.Resources;

using ImageEntity = global::Streetcode.DAL.Entities.Media.Images.Image;
using NewsEntity = global::Streetcode.DAL.Entities.News.News;

namespace Streetcode.XUnitTest.BLL.MediatR.News.GetByUrl
{
    public class GetNewsByUrlHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IBlobService> _blobServiceMock;
        private readonly Mock<ILoggerService> _loggerMock;

        private readonly GetNewsByUrlHandler _handler;

        public GetNewsByUrlHandlerTests()
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NewsProfile>();
                cfg.CreateMap<ImageEntity, ImageDTO>();
            }).CreateMapper();

            _repositoryWrapperMock = new Mock<IRepositoryWrapper> { DefaultValue = DefaultValue.Mock };
            _blobServiceMock = new Mock<IBlobService>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new GetNewsByUrlHandler(
                _mapper,
                _repositoryWrapperMock.Object,
                _blobServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenNewsNotFound()
        {
            string testUrl = "example-news-url";
            var request = new GetNewsByUrlQuery(testUrl);
            var errorMsg = string.Format(ErrorMessages.NoNewsFoundByUrl, testUrl);

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<NewsEntity, bool>>>(),
                It.IsAny<Func<IQueryable<NewsEntity>, IIncludableQueryable<NewsEntity, object>>>()))
                .ReturnsAsync((NewsEntity)null!);

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

            var newsEntity = new NewsEntity
            {
                Id = 1,
                Title = "Test News",
                Text = "This is a test news.",
                URL = testUrl,
                Image = null
            };

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<NewsEntity, bool>>>(),
                It.IsAny<Func<IQueryable<NewsEntity>, IIncludableQueryable<NewsEntity, object>>>()))
                .ReturnsAsync(newsEntity);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(testUrl, result.Value.URL);
            Assert.Null(result.Value.Image);
            _blobServiceMock.Verify(b => b.FindFileInStorageAsBase64(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnOkAndFetchBase64_WhenNewsHasImage()
        {
            string testUrl = "example-news-url";
            var request = new GetNewsByUrlQuery(testUrl);

            var newsEntity = new NewsEntity
            {
                Id = 1,
                Title = "Test News",
                Text = "This is a test news.",
                URL = testUrl,
                Image = new ImageEntity { BlobName = "test-image.jpg" }
            };
            var expectedBase64 = "base64-encoded-string";

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<NewsEntity, bool>>>(),
                It.IsAny<Func<IQueryable<NewsEntity>, IIncludableQueryable<NewsEntity, object>>>()))
                .ReturnsAsync(newsEntity);

            _blobServiceMock.Setup(b => b.FindFileInStorageAsBase64("test-image.jpg"))
                .Returns(expectedBase64);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(expectedBase64, result.Value.Image?.Base64);
            _blobServiceMock.Verify(b => b.FindFileInStorageAsBase64("test-image.jpg"), Times.Once);
        }
    }
}
