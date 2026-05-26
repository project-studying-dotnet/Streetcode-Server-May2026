using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.Media.Images;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Newss;
using Streetcode.BLL.MediatR.Newss.Update;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using Streetcode.BLL.Resources;

using NewsEntity = global::Streetcode.DAL.Entities.News.News;

namespace Streetcode.XUnitTest.BLL.MediatR.News.Update
{
    public class UpdateNewsHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<IBlobService> _blobServiceMock;
        private readonly Mock<ILoggerService> _loggerMock;

        private readonly UpdateNewsHandler _handler;

        public UpdateNewsHandlerTests()
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NewsProfile>();
                cfg.CreateMap<Image, ImageDTO>().ReverseMap();
            }).CreateMapper();

            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _blobServiceMock = new Mock<IBlobService>();
            _loggerMock = new Mock<ILoggerService>();

            _repositoryWrapperMock.Setup(r => r.NewsRepository.Update(It.IsAny<NewsEntity>()));
            _repositoryWrapperMock.Setup(r => r.ImageRepository.Delete(It.IsAny<Image>()));

            _handler = new UpdateNewsHandler(
                _repositoryWrapperMock.Object,
                _mapper,
                _blobServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenMapperReturnsNull()
        {
            var request = new UpdateNewsCommand(null!);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsFailed);
            Assert.Equal(ErrorMessages.CannotConvertNullToNews, result.Errors[0].Message);
            _loggerMock.Verify(l => l.LogError(request, ErrorMessages.CannotConvertNullToNews), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnOkAndFetchBase64_WhenImageIsNotNull()
        {
            var newsDto = new NewsDTO
            {
                Id = 1,
                Image = new ImageDTO { BlobName = "test.jpg" }
            };
            var request = new UpdateNewsCommand(newsDto);

            _blobServiceMock.Setup(b => b.FindFileInStorageAsBase64("test.jpg"))
                .Returns("base64-string");

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("base64-string", result.Value.Image.Base64);
            _repositoryWrapperMock.Verify(r => r.NewsRepository.Update(It.Is<NewsEntity>(n => n.Id == 1)), Times.Once);
            _blobServiceMock.Verify(b => b.FindFileInStorageAsBase64("test.jpg"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnOkAndDeleteOldImage_WhenImageIsNull_AndOldImageExists()
        {
            var newsDto = new NewsDTO { Id = 1, Image = null, ImageId = 99 };
            var request = new UpdateNewsCommand(newsDto);

            var oldImageEntity = new Image { Id = 99 };

            _repositoryWrapperMock.Setup(r => r.ImageRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Image, bool>>>(), null))
                .ReturnsAsync(oldImageEntity);

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _repositoryWrapperMock.Verify(r => r.ImageRepository.Delete(oldImageEntity), Times.Once);
            _repositoryWrapperMock.Verify(r => r.NewsRepository.Update(It.Is<NewsEntity>(n => n.Id == 1)), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnOkAndNotDelete_WhenImageIsNull_AndOldImageDoesNotExist()
        {
            var newsDto = new NewsDTO { Id = 1, Image = null, ImageId = 99 };
            var request = new UpdateNewsCommand(newsDto);

            _repositoryWrapperMock.Setup(r => r.ImageRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Image, bool>>>(), null))
                .ReturnsAsync((Image)null!);

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _repositoryWrapperMock.Verify(r => r.ImageRepository.Delete(It.IsAny<Image>()), Times.Never);
            _repositoryWrapperMock.Verify(r => r.NewsRepository.Update(It.Is<NewsEntity>(n => n.Id == 1)), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenSaveChangesReturnsZero()
        {
            var newsDto = new NewsDTO { Id = 1, Image = null };
            var request = new UpdateNewsCommand(newsDto);

            _repositoryWrapperMock.Setup(r => r.ImageRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Image, bool>>>(), null))
                .ReturnsAsync((Image)null!);

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsFailed);
            Assert.Equal(ErrorMessages.FailedToUpdateNews, result.Errors[0].Message);
            _loggerMock.Verify(l => l.LogError(request, ErrorMessages.FailedToUpdateNews), Times.Once);
        }
    }
}
