using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Newss.GetNewsAndLinksByUrl;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.News.GetNewsAndLinksByUrl
{
    public class GetNewsAndLinksByUrlHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IBlobService> _blobServiceMock;
        private readonly Mock<ILoggerService> _loggerMock;

        private readonly GetNewsAndLinksByUrlHandler _handler;

        public GetNewsAndLinksByUrlHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _repositoryWrapperMock = new Mock<IRepositoryWrapper> { DefaultValue = DefaultValue.Mock };
            _blobServiceMock = new Mock<IBlobService>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new GetNewsAndLinksByUrlHandler(
                _mapperMock.Object,
                _repositoryWrapperMock.Object,
                _blobServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenNewsNotFound()
        {
            string testUrl = "invalid-url";
            var request = new GetNewsAndLinksByUrlQuery(testUrl);

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<DAL.Entities.News.News, bool>>>(),
                It.IsAny<Func<IQueryable<DAL.Entities.News.News>, IIncludableQueryable<DAL.Entities.News.News, object>>>()))
                .ReturnsAsync((DAL.Entities.News.News)null);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsFailed);
            Assert.Equal($"No news by entered Url - {testUrl}", result.Errors[0].Message);
        }

        [Fact]
        public async Task Handle_ShouldReturnOk_WithCorrectLinks_WhenNewsCountIs3_AndIsFirstItem()
        {
            string targetUrl = "url1";
            var request = new GetNewsAndLinksByUrlQuery(targetUrl);

            var newsEntities = new List<DAL.Entities.News.News>
            {
                new DAL.Entities.News.News { Id = 1, URL = "url1", Title = "Title 1" },
                new DAL.Entities.News.News { Id = 2, URL = "url2", Title = "Title 2" },
                new DAL.Entities.News.News { Id = 3, URL = "url3", Title = "Title 3" }
            };

            var targetNewsDto = new NewsDTO
            {
                Id = 1,
                URL = "url1",
                Title = "Title 1",
                Image = new ImageDTO { BlobName = "test.jpg" }
            };

            SetupMocks(newsEntities, newsEntities[0], targetNewsDto);
            _blobServiceMock.Setup(b => b.FindFileInStorageAsBase64("test.jpg")).Returns("base64");

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("base64", result.Value.News.Image.Base64);
            Assert.Null(result.Value.PrevNewsUrl);
            Assert.Equal("url2", result.Value.NextNewsUrl);
            Assert.Equal("url1", result.Value.RandomNews.RandomNewsUrl);
        }

        [Fact]
        public async Task Handle_ShouldReturnOk_WithCorrectLinks_WhenNewsCountIs4_AndIsLastItem()
        {
            string targetUrl = "url4";
            var request = new GetNewsAndLinksByUrlQuery(targetUrl);

            var newsEntities = new List<DAL.Entities.News.News>
            {
                new DAL.Entities.News.News { Id = 1, URL = "url1", Title = "Title 1" },
                new DAL.Entities.News.News { Id = 2, URL = "url2", Title = "Title 2" },
                new DAL.Entities.News.News { Id = 3, URL = "url3", Title = "Title 3" },
                new DAL.Entities.News.News { Id = 4, URL = "url4", Title = "Title 4" },
            };

            var targetNewsDto = new NewsDTO { Id = 4, URL = "url4", Title = "Title 4", Image = null };

            SetupMocks(newsEntities, newsEntities[3], targetNewsDto);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("url3", result.Value.PrevNewsUrl);
            Assert.Null(result.Value.NextNewsUrl);
            Assert.Equal("url2", result.Value.RandomNews.RandomNewsUrl);
        }

        [Fact]
        public async Task Handle_ShouldReturnOk_WithCorrectLinks_WhenNewsCountIs5_AndIsFirstItem()
        {
            string targetUrl = "url1";
            var request = new GetNewsAndLinksByUrlQuery(targetUrl);

            var newsEntities = new List<DAL.Entities.News.News>
            {
                new DAL.Entities.News.News { Id = 1, URL = "url1", Title = "Title 1" },
                new DAL.Entities.News.News { Id = 2, URL = "url2", Title = "Title 2" },
                new DAL.Entities.News.News { Id = 3, URL = "url3", Title = "Title 3" },
                new DAL.Entities.News.News { Id = 4, URL = "url4", Title = "Title 4" },
                new DAL.Entities.News.News { Id = 5, URL = "url5", Title = "Title 5" },
            };

            var targetNewsDto = new NewsDTO { Id = 1, URL = "url1", Title = "Title 1", Image = null };

            SetupMocks(newsEntities, newsEntities[0], targetNewsDto);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("url5", result.Value.RandomNews.RandomNewsUrl);
        }

        private void SetupMocks(
            List<DAL.Entities.News.News> allNews,
            DAL.Entities.News.News targetEntity,
            NewsDTO targetDto)
        {
            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<DAL.Entities.News.News, bool>>>(),
                It.IsAny<Func<IQueryable<DAL.Entities.News.News>, IIncludableQueryable<DAL.Entities.News.News, object>>>()))
                .ReturnsAsync(targetEntity);

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetAllAsync(
                null, null))
                .ReturnsAsync(allNews);

            _mapperMock.Setup(m => m.Map<NewsDTO>(targetEntity))
                .Returns(targetDto);
        }
    }
}
