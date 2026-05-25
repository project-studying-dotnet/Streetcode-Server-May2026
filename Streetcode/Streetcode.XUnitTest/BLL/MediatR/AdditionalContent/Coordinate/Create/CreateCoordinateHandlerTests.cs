using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Create;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.AdditionalContent.Coordinate.Create;

public class CreateCoordinateHandlerTests
{
    private readonly Mock<IMapper> mapperMock;
    private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
    private readonly CreateCoordinateHandler handler;

    public CreateCoordinateHandlerTests()
    {
        mapperMock = new Mock<IMapper>();
        repositoryWrapperMock = new Mock<IRepositoryWrapper>();

        handler = new CreateCoordinateHandler(
            repositoryWrapperMock.Object,
            mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenMapperReturnsNull()
    {
        var command = new CreateCoordinateCommand(null!);

        mapperMock
            .Setup(m => m.Map<DAL.Entities.AdditionalContent.Coordinates.Types.StreetcodeCoordinate>(
                command.StreetcodeCoordinate))
            .Returns((DAL.Entities.AdditionalContent.Coordinates.Types.StreetcodeCoordinate?)null);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.CannotConvertNullToStreetcodeCoordinate);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
    {
        var command = new CreateCoordinateCommand(null!);

        var coordinate =
            new DAL.Entities.AdditionalContent.Coordinates.Types.StreetcodeCoordinate();

        mapperMock
            .Setup(m => m.Map<DAL.Entities.AdditionalContent.Coordinates.Types.StreetcodeCoordinate>(
                command.StreetcodeCoordinate))
            .Returns(coordinate);

        repositoryWrapperMock
            .Setup(r => r.StreetcodeCoordinateRepository.Create(coordinate));

        repositoryWrapperMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(0);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.FailedToCreateStreetcodeCoordinate);
    }
}