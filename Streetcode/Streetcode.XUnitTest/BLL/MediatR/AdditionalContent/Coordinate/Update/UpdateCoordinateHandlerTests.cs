using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Coordinates.Types;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Update;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Repositories.Interfaces.AdditionalContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.AdditionalContent.Coordinate.Update;

public class UpdateCoordinateHandlerTests
{
    private readonly Mock<IMapper> mapperMock;
    private readonly Mock<IRepositoryWrapper> repositoryWrapperMock;
    private readonly Mock<IStreetcodeCoordinateRepository> coordinateRepositoryMock;
    private readonly UpdateCoordinateHandler handler;

    public UpdateCoordinateHandlerTests()
    {
        mapperMock = new Mock<IMapper>();
        repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        coordinateRepositoryMock = new Mock<IStreetcodeCoordinateRepository>();

        repositoryWrapperMock
            .Setup(r => r.StreetcodeCoordinateRepository)
            .Returns(coordinateRepositoryMock.Object);

        handler = new UpdateCoordinateHandler(
            repositoryWrapperMock.Object,
            mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenMapperReturnsNull()
    {
        var command = new UpdateCoordinateCommand(null!);

        mapperMock
            .Setup(m => m.Map<StreetcodeCoordinate>(
                command.StreetcodeCoordinate))
            .Returns((StreetcodeCoordinate?)null);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .Be(ErrorMessages.CannotConvertNullToStreetcodeCoordinate);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenUpdateFails()
    {
        var coordinateDto = new StreetcodeCoordinateDTO();
        var command = new UpdateCoordinateCommand(coordinateDto);
        var coordinate = new StreetcodeCoordinate();

        mapperMock
            .Setup(m => m.Map<StreetcodeCoordinate>(coordinateDto))
            .Returns(coordinate);

        repositoryWrapperMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(0);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .Be(ErrorMessages.FailedToUpdateStreetcodeCoordinate);

        coordinateRepositoryMock.Verify(
            r => r.Update(coordinate),
            Times.Once);

        repositoryWrapperMock.Verify(
            r => r.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCoordinateUpdated()
    {
        var coordinateDto = new StreetcodeCoordinateDTO();
        var command = new UpdateCoordinateCommand(coordinateDto);
        var coordinate = new StreetcodeCoordinate();

        mapperMock
            .Setup(m => m.Map<StreetcodeCoordinate>(coordinateDto))
            .Returns(coordinate);

        repositoryWrapperMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        coordinateRepositoryMock.Verify(
            r => r.Update(coordinate),
            Times.Once);

        repositoryWrapperMock.Verify(
            r => r.SaveChangesAsync(),
            Times.Once);
    }
}