using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Newss.GetAll;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;

namespace Streetcode.XUnitTest.Features.News.GetAll
{
    public class GetAllNewsHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IBlobService> _blobServiceMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly GetAllNewsHandler _handler;

        public GetAllNewsHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper> { DefaultValue = DefaultValue.Mock };
            _mapperMock = new Mock<IMapper>();
            _blobServiceMock = new Mock<IBlobService>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new GetAllNewsHandler(
                _repositoryWrapperMock.Object,
                _mapperMock.Object,
                _blobServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenNewsIsNull()
        {
            var request = new GetAllNewsQuery();

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetAllAsync(
                It.IsAny<Expression<Func<Streetcode.DAL.Entities.News.News, bool>>>(),
                It.IsAny<Func<IQueryable<Streetcode.DAL.Entities.News.News>, IIncludableQueryable<Streetcode.DAL.Entities.News.News, object>>>()))
                .ReturnsAsync((IEnumerable<Streetcode.DAL.Entities.News.News>)null);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsFailed);
            Assert.Equal("There are no news in the database", result.Errors[0].Message);
            _loggerMock.Verify(l => l.LogError(request, "There are no news in the database"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnOk_WhenNewsHasNoImages()
        {
            var request = new GetAllNewsQuery();
            var newsEntities = new List<Streetcode.DAL.Entities.News.News> { new Streetcode.DAL.Entities.News.News { Id = 1 } };

            var newsDTOs = new List<NewsDTO> { new NewsDTO { Id = 1, Image = null } };

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetAllAsync(
                It.IsAny<Expression<Func<Streetcode.DAL.Entities.News.News, bool>>>(),
                It.IsAny<Func<IQueryable<Streetcode.DAL.Entities.News.News>, IIncludableQueryable<Streetcode.DAL.Entities.News.News, object>>>()))
                .ReturnsAsync(newsEntities);

            _mapperMock.Setup(m => m.Map<IEnumerable<NewsDTO>>(It.IsAny<IEnumerable<Streetcode.DAL.Entities.News.News>>()))
                .Returns(newsDTOs);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(newsDTOs, result.Value);

            _blobServiceMock.Verify(b => b.FindFileInStorageAsBase64(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnOkAndFetchBase64_WhenNewsHasImages()
        {
            var request = new GetAllNewsQuery();
            var newsEntities = new List<Streetcode.DAL.Entities.News.News> { new Streetcode.DAL.Entities.News.News { Id = 1 } };

            var newsDTOs = new List<NewsDTO>
            {
                new NewsDTO
                {
                    Id = 1,
                    Image = new ImageDTO { BlobName = "test-image.jpg" }
                }
            };

            var expectedBase64 = "base64-encoded-string";

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetAllAsync(
                It.IsAny<Expression<Func<Streetcode.DAL.Entities.News.News, bool>>>(),
                It.IsAny<Func<IQueryable<Streetcode.DAL.Entities.News.News>, IIncludableQueryable<Streetcode.DAL.Entities.News.News, object>>>()))
                .ReturnsAsync(newsEntities);

            _mapperMock.Setup(m => m.Map<IEnumerable<NewsDTO>>(It.IsAny<IEnumerable<Streetcode.DAL.Entities.News.News>>()))
                .Returns(newsDTOs);

            _blobServiceMock.Setup(b => b.FindFileInStorageAsBase64("test-image.jpg"))
                .Returns(expectedBase64);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);

            _blobServiceMock.Verify(b => b.FindFileInStorageAsBase64("test-image.jpg"), Times.Once);

            Assert.Equal(expectedBase64, result.Value.First().Image.Base64);
        }
    }
}
