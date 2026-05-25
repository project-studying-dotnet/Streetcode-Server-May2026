using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.News;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Media.Images;
using Streetcode.BLL.Mapping.Newss;
using Streetcode.BLL.MediatR.Newss.Create;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using Streetcode.BLL.Resources;


namespace Streetcode.XUnitTest.BLL.MediatR.News.Create
{
    public class CreateNewsHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;

        private readonly CreateNewsHandler _handler;

        public CreateNewsHandlerTests()
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NewsProfile>();
                cfg.AddProfile<ImageProfile>();
            }).CreateMapper();

            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();

            _handler = new CreateNewsHandler(
                _mapper,
                _repositoryWrapperMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenMapperReturnsNull()
        {
            var request = new CreateNewsCommand(null);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsFailed);
            Assert.Equal(ErrorMessages.CannotConvertNullToNews, result.Errors[0].Message);

            _loggerMock.Verify(l => l.LogError(request, ErrorMessages.CannotConvertNullToNews), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenNewsIsCreated()
        {
            var newsDto = new NewsDTO { Title = "Test Title", ImageId = 1 };
            var request = new CreateNewsCommand(newsDto);

            _repositoryWrapperMock.Setup(r => r.NewsRepository.Create(It.IsAny<DAL.Entities.News.News>()))
                                  .Returns((DAL.Entities.News.News n) => n);

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
                                  .ReturnsAsync(1);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);

            Assert.Equal(newsDto.Title, result.Value.Title);
            Assert.Equal(newsDto.ImageId, result.Value.ImageId);

            _repositoryWrapperMock.Verify(r => r.NewsRepository.Create(It.IsAny<DAL.Entities.News.News>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFail_WhenSaveChangesReturnsZero()
        {
            var request = new CreateNewsCommand(new NewsDTO { ImageId = 0 });

            DAL.Entities.News.News capturedEntity = null;

            _repositoryWrapperMock.Setup(r => r.NewsRepository.Create(It.IsAny<DAL.Entities.News.News>()))
                                  .Callback<DAL.Entities.News.News>(n => capturedEntity = n)
                                  .Returns((DAL.Entities.News.News n) => n);

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
                                  .ReturnsAsync(0);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsFailed);
            Assert.Equal(ErrorMessages.FailedToCreateNews, result.Errors[0].Message);

            _loggerMock.Verify(l => l.LogError(request, ErrorMessages.FailedToCreateNews), Times.Once);

            Assert.NotNull(capturedEntity);
            Assert.Null(capturedEntity.ImageId);
        }
    }
}
