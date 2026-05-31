using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Media.Images;
using Streetcode.BLL.Mapping.Newss;
using Streetcode.BLL.MediatR.Newss.GetNewsAndLinksByUrl;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using Streetcode.BLL.Resources;

using ImageEntity = global::Streetcode.DAL.Entities.Media.Images.Image;
using NewsEntity = global::Streetcode.DAL.Entities.News.News;

namespace Streetcode.XUnitTest.BLL.MediatR.News.GetNewsAndLinksByUrl
{
    public class GetNewsAndLinksByUrlHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IBlobService> _blobServiceMock;
        private readonly Mock<ILoggerService> _loggerMock;

        private readonly GetNewsAndLinksByUrlHandler _handler;

        public GetNewsAndLinksByUrlHandlerTests()
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NewsProfile>();
                cfg.AddProfile<ImageProfile>();
            }).CreateMapper();

            _repositoryWrapperMock = new Mock<IRepositoryWrapper> { DefaultValue = DefaultValue.Mock };
            _blobServiceMock = new Mock<IBlobService>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new GetNewsAndLinksByUrlHandler(
                _mapper,
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
                It.IsAny<Expression<Func<NewsEntity, bool>>>(),
                It.IsAny<Func<IQueryable<NewsEntity>, IIncludableQueryable<NewsEntity, object>>>()))
                .ReturnsAsync((NewsEntity)null!);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsFailed);
            Assert.Equal(string.Format(ErrorMessages.NoNewsFoundByUrl, testUrl), result.Errors[0].Message);
        }

        [Fact]
        public async Task Handle_ShouldReturnOk_WithCorrectLinks_WhenNewsCountIs3_AndIsFirstItem()
        {
            string targetUrl = "url1";
            var request = new GetNewsAndLinksByUrlQuery(targetUrl);

            var newsEntities = new List<NewsEntity>
            {
                new NewsEntity
                {
                    Id = 1,
                    URL = "url1",
                    Title = "Title 1",
                    Text = "Text 1",
                    Image = new ImageEntity { BlobName = "test.jpg" }
                },
                new NewsEntity { Id = 2, URL = "url2", Title = "Title 2", Text = "Text 2" },
                new NewsEntity { Id = 3, URL = "url3", Title = "Title 3", Text = "Text 3" }
            };

            SetupMocks(newsEntities, newsEntities[0]);
            _blobServiceMock.Setup(b => b.FindFileInStorageAsBase64("test.jpg")).Returns("base64");

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("base64", result.Value.News.Image?.Base64);
            Assert.Null(result.Value.PrevNewsUrl);
            Assert.Equal("url2", result.Value.NextNewsUrl);
            Assert.Equal("url1", result.Value.RandomNews?.RandomNewsUrl);
        }

        [Fact]
        public async Task Handle_ShouldReturnOk_WithCorrectLinks_WhenNewsCountIs4_AndIsLastItem()
        {
            string targetUrl = "url4";
            var request = new GetNewsAndLinksByUrlQuery(targetUrl);

            var newsEntities = new List<NewsEntity>
            {
                new NewsEntity { Id = 1, URL = "url1", Title = "Title 1", Text = "Text 1" },
                new NewsEntity { Id = 2, URL = "url2", Title = "Title 2", Text = "Text 2" },
                new NewsEntity { Id = 3, URL = "url3", Title = "Title 3", Text = "Text 3" },
                new NewsEntity { Id = 4, URL = "url4", Title = "Title 4", Text = "Text 4" },
            };

            SetupMocks(newsEntities, newsEntities[3]);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("url3", result.Value.PrevNewsUrl);
            Assert.Null(result.Value.NextNewsUrl);
            Assert.Equal("url2", result.Value.RandomNews?.RandomNewsUrl);
        }

        [Fact]
        public async Task Handle_ShouldReturnOk_WithCorrectLinks_WhenNewsCountIs5_AndIsFirstItem()
        {
            string targetUrl = "url1";
            var request = new GetNewsAndLinksByUrlQuery(targetUrl);

            var newsEntities = new List<NewsEntity>
            {
                new NewsEntity { Id = 1, URL = "url1", Title = "Title 1", Text = "Text 1" },
                new NewsEntity { Id = 2, URL = "url2", Title = "Title 2", Text = "Text 2" },
                new NewsEntity { Id = 3, URL = "url3", Title = "Title 3", Text = "Text 3" },
                new NewsEntity { Id = 4, URL = "url4", Title = "Title 4", Text = "Text 4" },
                new NewsEntity { Id = 5, URL = "url5", Title = "Title 5", Text = "Text 5" },
            };

            SetupMocks(newsEntities, newsEntities[0]);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("url5", result.Value.RandomNews?.RandomNewsUrl);
        }

        private void SetupMocks(
            List<NewsEntity> allNews,
            NewsEntity targetEntity)
        {
            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<NewsEntity, bool>>>(),
                It.IsAny<Func<IQueryable<NewsEntity>, IIncludableQueryable<NewsEntity, object>>>()))
                .ReturnsAsync(targetEntity);

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetAllAsync(
                null, null))
                .ReturnsAsync(allNews);
        }
    }
}
