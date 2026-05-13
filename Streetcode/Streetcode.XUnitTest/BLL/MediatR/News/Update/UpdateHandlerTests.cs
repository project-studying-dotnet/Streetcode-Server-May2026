using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Newss.Update;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.News.Update
{
    public class UpdateNewsHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IBlobService> _blobServiceMock;
        private readonly Mock<ILoggerService> _loggerMock;

        private readonly UpdateNewsHandler _handler;

        public UpdateNewsHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _mapperMock = new Mock<IMapper>();
            _blobServiceMock = new Mock<IBlobService>();
            _loggerMock = new Mock<ILoggerService>();

            _repositoryWrapperMock.Setup(r => r.NewsRepository.Update(It.IsAny<DAL.Entities.News.News>()));
            _repositoryWrapperMock.Setup(r => r.ImageRepository.Delete(It.IsAny<Image>()));

            _handler = new UpdateNewsHandler(
                _repositoryWrapperMock.Object,
                _mapperMock.Object,
                _blobServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenMapperReturnsNull()
        {
            var request = new UpdateNewsCommand(new NewsDTO());

            _mapperMock.Setup(m => m.Map<DAL.Entities.News.News>(It.IsAny<NewsDTO>()))
                .Returns((DAL.Entities.News.News)null);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsFailed);
            Assert.Equal("Cannot convert null to news", result.Errors[0].Message);
            _loggerMock.Verify(l => l.LogError(request, "Cannot convert null to news"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnOkAndFetchBase64_WhenImageIsNotNull()
        {
            var newsDto = new NewsDTO { Id = 1 };
            var request = new UpdateNewsCommand(newsDto);

            var newsEntity = new DAL.Entities.News.News
            {
                Id = 1,
                Image = new Image()
            };

            var responseDto = new NewsDTO
            {
                Id = 1,
                Image = new ImageDTO { BlobName = "test.jpg" }
            };

            _mapperMock.Setup(m => m.Map<DAL.Entities.News.News>(It.IsAny<NewsDTO>()))
                .Returns(newsEntity);
            _mapperMock.Setup(m => m.Map<NewsDTO>(newsEntity))
                .Returns(responseDto);

            _blobServiceMock.Setup(b => b.FindFileInStorageAsBase64("test.jpg"))
                .Returns("base64-string");

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("base64-string", result.Value.Image.Base64);
            _repositoryWrapperMock.Verify(r => r.NewsRepository.Update(newsEntity), Times.Once);
            _blobServiceMock.Verify(b => b.FindFileInStorageAsBase64("test.jpg"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnOkAndDeleteOldImage_WhenImageIsNull_AndOldImageExists()
        {
            var newsDto = new NewsDTO { Id = 1 };
            var request = new UpdateNewsCommand(newsDto);

            var newsEntity = new DAL.Entities.News.News { Id = 1, Image = null };
            var responseDto = new NewsDTO { Id = 1, ImageId = 99 };
            var oldImageEntity = new Image { Id = 99 };

            _mapperMock.Setup(m => m.Map<DAL.Entities.News.News>(It.IsAny<NewsDTO>()))
                .Returns(newsEntity);
            _mapperMock.Setup(m => m.Map<NewsDTO>(newsEntity))
                .Returns(responseDto);

            _repositoryWrapperMock.Setup(r => r.ImageRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Image, bool>>>(), null))
                .ReturnsAsync(oldImageEntity);

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _repositoryWrapperMock.Verify(r => r.ImageRepository.Delete(oldImageEntity), Times.Once);
            _repositoryWrapperMock.Verify(r => r.NewsRepository.Update(newsEntity), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnOkAndNotDelete_WhenImageIsNull_AndOldImageDoesNotExist()
        {
            var request = new UpdateNewsCommand(new NewsDTO());
            var newsEntity = new DAL.Entities.News.News { Id = 1, Image = null };
            var responseDto = new NewsDTO { Id = 1, ImageId = 99 };

            _mapperMock.Setup(m => m.Map<DAL.Entities.News.News>(It.IsAny<NewsDTO>()))
                .Returns(newsEntity);
            _mapperMock.Setup(m => m.Map<NewsDTO>(newsEntity))
                .Returns(responseDto);

            _repositoryWrapperMock.Setup(r => r.ImageRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Image, bool>>>(), null))
                .ReturnsAsync((Image)null);

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _repositoryWrapperMock.Verify(r => r.ImageRepository.Delete(It.IsAny<Image>()), Times.Never);
            _repositoryWrapperMock.Verify(r => r.NewsRepository.Update(newsEntity), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenSaveChangesReturnsZero()
        {
            var request = new UpdateNewsCommand(new NewsDTO());
            var newsEntity = new DAL.Entities.News.News { Id = 1, Image = null };
            var responseDto = new NewsDTO { Id = 1 };

            _mapperMock.Setup(m => m.Map<DAL.Entities.News.News>(It.IsAny<NewsDTO>()))
                .Returns(newsEntity);
            _mapperMock.Setup(m => m.Map<NewsDTO>(newsEntity))
                .Returns(responseDto);

            _repositoryWrapperMock.Setup(r => r.ImageRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Image, bool>>>(), null))
                .ReturnsAsync((Image)null);

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsFailed);
            Assert.Equal("Failed to update news", result.Errors[0].Message);
            _loggerMock.Verify(l => l.LogError(request, "Failed to update news"), Times.Once);
        }
    }
}
