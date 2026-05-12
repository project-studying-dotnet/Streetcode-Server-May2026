using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Newss.Delete;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Linq.Expressions;
using Xunit;

namespace Streetcode.XUnitTest.Features.News.Delete
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
                It.IsAny<Expression<Func<Streetcode.DAL.Entities.News.News, bool>>>(), null))
                .ReturnsAsync((Streetcode.DAL.Entities.News.News)null);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsFailed);
            Assert.Equal("No news found by entered Id - 1", result.Errors[0].Message);
            _loggerMock.Verify(l => l.LogError(request, "No news found by entered Id - 1"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldDeleteNewsAndImage_WhenImageExists()
        {
            var request = new DeleteNewsCommand(1);
            var newsEntity = new Streetcode.DAL.Entities.News.News
            {
                Id = 1,
                Image = new Image()
            };

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Streetcode.DAL.Entities.News.News, bool>>>(), null))
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
            var newsEntity = new Streetcode.DAL.Entities.News.News
            {
                Id = 1,
                Image = null
            };

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Streetcode.DAL.Entities.News.News, bool>>>(), null))
                .ReturnsAsync(newsEntity);

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            _repositoryWrapperMock.Verify(r => r.NewsRepository.Delete(newsEntity), Times.Once);
            _repositoryWrapperMock.Verify(r => r.ImageRepository.Delete(It.IsAny<Image>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenSaveChangesReturnsZero()
        {
            var request = new DeleteNewsCommand(1);
            var newsEntity = new Streetcode.DAL.Entities.News.News { Id = 1 };

            _repositoryWrapperMock.Setup(r => r.NewsRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<Streetcode.DAL.Entities.News.News, bool>>>(), null))
                .ReturnsAsync(newsEntity);

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(0);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsFailed);
            Assert.Equal("Failed to delete news", result.Errors[0].Message);
            _loggerMock.Verify(l => l.LogError(request, "Failed to delete news"), Times.Once);
        }
    }
}