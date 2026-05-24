using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Delete;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.AdditionalContent.Coordinate.Delete
{
    public class DeleteCoordinateHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
        private readonly DeleteCoordinateHandler _handler;

        public DeleteCoordinateHandlerTests()
        {
            _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
            _handler = new DeleteCoordinateHandler(_repositoryWrapperMock.Object);
        }

        [Fact]
        public async Task Handle_CoordinateExists_ReturnsOkResult()
        {
            var command = new DeleteCoordinateCommand(1);
            var coordinate = new StreetcodeCoordinate { Id = 1 };

            _repositoryWrapperMock.Setup(r => r.StreetcodeCoordinateRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeCoordinate, bool>>>(), null))
                .ReturnsAsync(coordinate);

            _repositoryWrapperMock.Setup(r => r.StreetcodeCoordinateRepository.Delete(coordinate));

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_CoordinateDoesNotExist_ReturnsFailResult()
        {
            var command = new DeleteCoordinateCommand(1);

            _repositoryWrapperMock.Setup(r => r.StreetcodeCoordinateRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeCoordinate, bool>>>(), null))
                .ReturnsAsync((StreetcodeCoordinate)null!);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Equal($"Cannot find a coordinate with corresponding categoryId: {command.Id}", result.Errors.First().Message);
        }

        [Fact]
        public async Task Handle_SaveChangesFails_ReturnsFailResult()
        {
            var command = new DeleteCoordinateCommand(1);
            var coordinate = new StreetcodeCoordinate { Id = 1 };

            _repositoryWrapperMock.Setup(r => r.StreetcodeCoordinateRepository.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<StreetcodeCoordinate, bool>>>(), null))
                .ReturnsAsync(coordinate);

            _repositoryWrapperMock.Setup(r => r.StreetcodeCoordinateRepository.Delete(coordinate));

            _repositoryWrapperMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(0);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
        }
    }
}