using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Media.Images;
using Streetcode.BLL.Mapping.Newss;
using Streetcode.BLL.MediatR.Newss.SortedByDateTime;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.News.SortedByDateTime
{
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

            _repositoryWrapperMock = new Mock<IRepositoryWrapper> { DefaultValue = DefaultValue.Mock };
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

            var newsEntities = new List<Streetcode.DAL.Entities.News.News>
            {
                new DAL.Entities.News.News
                {
                    Id = 1,
                    CreationDate = new DateTime(2020, 1, 1),
                    Image = null,
                },
                new DAL.Entities.News.News
                {
                    Id = 2,
                    CreationDate = new DateTime(2023, 1, 1),
                    Image = new DAL.Entities.Media.Images.Image { BlobName = "test.jpg" },
                },
            };

            var expectedBase64 = "base64-encoded-string";

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetAllAsync(
                It.IsAny<Expression<Func<DAL.Entities.News.News, bool>>>(),
                It.IsAny<Func<IQueryable<DAL.Entities.News.News>, IIncludableQueryable<DAL.Entities.News.News, object>>>()))
                .ReturnsAsync(newsEntities);

            _blobServiceMock.Setup(b => b.FindFileInStorageAsBase64("test.jpg"))
                .Returns(expectedBase64);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Value.Count);

            Assert.Equal(2, result.Value[0].Id);
            Assert.Equal(1, result.Value[1].Id);

            Assert.Equal(expectedBase64, result.Value[0].Image.Base64);
        }
    }
}
