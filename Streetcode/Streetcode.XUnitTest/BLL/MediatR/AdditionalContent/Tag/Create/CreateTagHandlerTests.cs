using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Tag;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.AdditionalContent.Tag.Create;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.AdditionalContent.Tag.Create
{
    public class CreateTagHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly Mock<ILoggerService> _loggerMock;
        private readonly CreateTagHandler _handler;

        public CreateTagHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _loggerMock = new Mock<ILoggerService>();
            _handler = new CreateTagHandler(_repositoryWrapperMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsOkResult()
        {
            var dto = new CreateTagDTO { Title = "Test" };
            var request = new CreateTagQuery(dto);
            var tag = new Streetcode.DAL.Entities.AdditionalContent.Tag();

            _mapperMock.Setup(m => m.Map<Streetcode.DAL.Entities.AdditionalContent.Tag>(It.IsAny<CreateTagDTO>()))
                .Returns(tag);

            _repositoryWrapperMock.Setup(r => r.TagRepository.Create(tag));

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            var result = await _handler.Handle(request, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }
    }
}