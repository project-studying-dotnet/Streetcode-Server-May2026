using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Newss.Create;
using Streetcode.DAL.Entities.News;
using Streetcode.DAL.Repositories.Interfaces.Base;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Xunit;

namespace Streetcode.XUnitTest.Features.News.Create
{
    public class CreateNewsHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;

        private readonly CreateNewsHandler _handler;

        public CreateNewsHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new CreateNewsHandler(
                _mapperMock.Object,
                _repositoryWrapperMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenMapperReturnsNull()
        {
            var request = new CreateNewsCommand(new NewsDTO());

            _mapperMock.Setup(m => m.Map<Streetcode.DAL.Entities.News.News>(It.IsAny<NewsDTO>()))
                       .Returns((Streetcode.DAL.Entities.News.News)null);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsFailed);
            Assert.Equal("Cannot convert null to news", result.Errors[0].Message);

            _loggerMock.Verify(l => l.LogError(request, "Cannot convert null to news"), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenNewsIsCreated()
        {
            var newsDto = new NewsDTO();
            var request = new CreateNewsCommand(newsDto);
            var newsEntity = new Streetcode.DAL.Entities.News.News { ImageId = 1 };

            _mapperMock.Setup(m => m.Map<Streetcode.DAL.Entities.News.News>(It.IsAny<NewsDTO>()))
                       .Returns(newsEntity);
            _mapperMock.Setup(m => m.Map<NewsDTO>(It.IsAny<Streetcode.DAL.Entities.News.News>()))
                       .Returns(newsDto);

            _repositoryWrapperMock.Setup(r => r.NewsRepository.Create(It.IsAny<Streetcode.DAL.Entities.News.News>()))
                                  .Returns(newsEntity);

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
                                  .ReturnsAsync(1);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(newsDto, result.Value);

            _repositoryWrapperMock.Verify(r => r.NewsRepository.Create(It.IsAny<Streetcode.DAL.Entities.News.News>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenSaveChangesReturnsZero()
        {
            var request = new CreateNewsCommand(new NewsDTO());
            var newsEntity = new Streetcode.DAL.Entities.News.News { ImageId = 0 };

            _mapperMock.Setup(m => m.Map<Streetcode.DAL.Entities.News.News>(It.IsAny<NewsDTO>()))
                       .Returns(newsEntity);

            _repositoryWrapperMock.Setup(r => r.NewsRepository.Create(It.IsAny<Streetcode.DAL.Entities.News.News>()))
                                  .Returns(newsEntity);

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
                                  .ReturnsAsync(0);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsFailed);
            Assert.Equal("Failed to create a news", result.Errors[0].Message);

            _loggerMock.Verify(l => l.LogError(request, "Failed to create a news"), Times.Once);

            Assert.Null(newsEntity.ImageId);
        }
    }
}