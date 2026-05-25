using FluentAssertions;
using Moq;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Delete;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.AdditionalContent.Coordinate.Delete;

public class DeleteCoordinateHandlerTests
{
    private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
    private readonly DeleteCoordinateHandler handler;

    public DeleteCoordinateHandlerTests()
    {
        repositoryWrapperMock = new Mock<IRepositoryWrapper>();

        handler = new DeleteCoordinateHandler(
            repositoryWrapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenCoordinateDoesNotExist()
    {
        var command = new DeleteCoordinateCommand(1);

        repositoryWrapperMock
            .Setup(r => r.StreetcodeCoordinateRepository.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<StreetcodeCoordinate, bool>>>(),
                null))
            .ReturnsAsync((StreetcodeCoordinate?)null);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(
            string.Format(ErrorMessages.CannotFindCoordinateByCategoryId, command.Id));
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
    {
        var command = new DeleteCoordinateCommand(1);
        var coordinate = new StreetcodeCoordinate { Id = command.Id };

        repositoryWrapperMock
            .Setup(r => r.StreetcodeCoordinateRepository.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<StreetcodeCoordinate, bool>>>(),
                null))
            .ReturnsAsync(coordinate);

        repositoryWrapperMock
            .Setup(r => r.StreetcodeCoordinateRepository.Delete(coordinate));

        repositoryWrapperMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(0);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.FailedToDeleteCoordinate);
    }
}