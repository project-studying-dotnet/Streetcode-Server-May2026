using System.Linq.Expressions;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Newss.Delete;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using Streetcode.BLL.Resources;

using ImageEntity = global::Streetcode.DAL.Entities.Media.Images.Image;
using NewsEntity = global::Streetcode.DAL.Entities.News.News;

namespace Streetcode.XUnitTest.BLL.MediatR.News.Delete
{
    public class DeleteNewsHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;

        private readonly DeleteNewsHandler _handler;

        public DeleteNewsHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper> { DefaultValue = DefaultValue.Mock };
            _loggerMock = new Mock<ILoggerService>();

            _handler = new DeleteNewsHandler(
                _repositoryWrapperMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenNewsNotFound()
        {
            var request = new DeleteNewsCommand(1);

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<NewsEntity, bool>>>(), null))
                .ReturnsAsync((NewsEntity)null!);

            var result = await _handler.Handle(request, CancellationToken.None);
            const int idMessages = 1;

            Assert.True(result.IsFailed);
            Assert.Equal(string.Format(ErrorMessages.NoNewsFoundById, idMessages), result.Errors[0].Message);
            _loggerMock.Verify(l => l.LogError(request, string.Format(ErrorMessages.NoNewsFoundById, idMessages)), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldDeleteNewsAndImage_WhenImageExists()
        {
            var request = new DeleteNewsCommand(1);
            var newsEntity = new NewsEntity
            {
                Id = 1,
                Image = new ImageEntity(),
                Title = "Test News",
                Text = "This is a test news.",
                URL = "test-url",
            };

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<NewsEntity, bool>>>(), null))
                .ReturnsAsync(newsEntity);

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _repositoryWrapperMock.Verify(r => r.ImageRepository.Delete(newsEntity.Image), Times.Once);
            _repositoryWrapperMock.Verify(r => r.NewsRepository.Delete(newsEntity), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldDeleteOnlyNews_WhenImageIsNull()
        {
            var request = new DeleteNewsCommand(1);
            var newsEntity = new NewsEntity
            {
                Id = 1,
                Image = null,
                Title = "Test News",
                Text = "This is a test news.",
                URL = "test-url"
            };

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<NewsEntity, bool>>>(), null))
                .ReturnsAsync(newsEntity);

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _repositoryWrapperMock.Verify(r => r.NewsRepository.Delete(newsEntity), Times.Once);
            _repositoryWrapperMock.Verify(r => r.ImageRepository.Delete(It.IsAny<ImageEntity>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenSaveChangesReturnsZero()
        {
            var request = new DeleteNewsCommand(1);
            var newsEntity = new NewsEntity
            {
                Id = 1,
                Image = new ImageEntity(),
                Title = "Test News",
                Text = "This is a test news.",
                URL = "test-url"
            };

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<NewsEntity, bool>>>(), null))
                .ReturnsAsync(newsEntity);

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsFailed);
            Assert.Equal(ErrorMessages.FailedToDeleteNews, result.Errors[0].Message);
            _loggerMock.Verify(l => l.LogError(request, ErrorMessages.FailedToDeleteNews), Times.Once);
        }
    }
}
