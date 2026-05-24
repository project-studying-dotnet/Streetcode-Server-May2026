using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Create;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.AdditionalContent.Coordinate.Create
{
    public class CreateCoordinateHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly CreateCoordinateHandler _handler;

        public CreateCoordinateHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _handler = new CreateCoordinateHandler(_repositoryWrapperMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsOkResult()
        {
            var command = new CreateCoordinateCommand(null!);
            var coordinate = new StreetcodeCoordinate();

            _mapperMock.Setup(m => m.Map<StreetcodeCoordinate>(It.IsAny<object>()))
                .Returns(coordinate);

            _repositoryWrapperMock.Setup(r => r.StreetcodeCoordinateRepository.Create(coordinate));

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_MapperReturnsNull_ReturnsFailResult()
        {
            var command = new CreateCoordinateCommand(null!);

            _mapperMock.Setup(m => m.Map<StreetcodeCoordinate>(It.IsAny<object>()))
                .Returns((StreetcodeCoordinate)null!);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal("Cannot convert null to streetcodeCoordinate", result.Errors.First().Message);
        }

        [Fact]
        public async Task Handle_SaveChangesFails_ReturnsFailResult()
        {
            var command = new CreateCoordinateCommand(null!);
            var coordinate = new StreetcodeCoordinate();

            _mapperMock.Setup(m => m.Map<StreetcodeCoordinate>(It.IsAny<object>()))
                .Returns(coordinate);

            _repositoryWrapperMock.Setup(r => r.StreetcodeCoordinateRepository.Create(coordinate));

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(0);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
        }
    }
}