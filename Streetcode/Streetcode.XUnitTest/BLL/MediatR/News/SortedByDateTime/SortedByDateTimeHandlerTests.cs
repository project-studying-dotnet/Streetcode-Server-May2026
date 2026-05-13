using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Newss.SortedByDateTime;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.News.SortedByDateTime
{
    public class SortedByDateTimeHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IBlobService> _blobServiceMock;
        private readonly Mock<ILoggerService> _loggerMock;

        private readonly SortedByDateTimeHandler _handler;

        public SortedByDateTimeHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper> { DefaultValue = DefaultValue.Mock };
            _mapperMock = new Mock<IMapper>();
            _blobServiceMock = new Mock<IBlobService>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new SortedByDateTimeHandler(
                _repositoryWrapperMock.Object,
                _mapperMock.Object,
                _blobServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenNewsIsNull()
        {
            var request = new SortedByDateTimeQuery();

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetAllAsync(
                It.IsAny<Expression<Func<DAL.Entities.News.News, bool>>>(),
                It.IsAny<Func<IQueryable<DAL.Entities.News.News>, IIncludableQueryable<DAL.Entities.News.News, object>>>()))
                .ReturnsAsync((IEnumerable<DAL.Entities.News.News>)null);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsFailed);
            Assert.Equal("There are no news in the database", result.Errors[0].Message);
            _loggerMock.Verify(l => l.LogError(request, "There are no news in the database"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnOkAndSortDescending_WhenNewsExist()
        {
            var request = new SortedByDateTimeQuery();
            var newsEntities = new List<DAL.Entities.News.News>
            {
                new DAL.Entities.News.News { Id = 1 },
                new DAL.Entities.News.News { Id = 2 }
            };

            var oldNewsDto = new NewsDTO
            {
                Id = 1,
                CreationDate = new DateTime(2020, 1, 1),
                Image = null,
            };
            var newNewsDto = new NewsDTO
            {
                Id = 2,
                CreationDate = new DateTime(2023, 1, 1),
                Image = new ImageDTO { BlobName = "test.jpg" },
            };

            var expectedBase64 = "base64-encoded-string";

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetAllAsync(
                It.IsAny<Expression<Func<DAL.Entities.News.News, bool>>>(),
                It.IsAny<Func<IQueryable<DAL.Entities.News.News>, IIncludableQueryable<DAL.Entities.News.News, object>>>()))
                .ReturnsAsync(newsEntities);

            _mapperMock.Setup(m => m.Map<IEnumerable<NewsDTO>>(newsEntities))
                .Returns(new List<NewsDTO> { oldNewsDto, newNewsDto });

            _blobServiceMock.Setup(b => b.FindFileInStorageAsBase64("test.jpg"))
                .Returns(expectedBase64);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count);

            Assert.Equal(2, result.Value[0].Id);
            Assert.Equal(1, result.Value[1].Id);

            Assert.Equal(expectedBase64, result.Value[0].Image.Base64);
            _blobServiceMock.Verify(b => b.FindFileInStorageAsBase64("test.jpg"), Times.Once);
        }
    }
}
