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
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly Mock<IStreetcodeCoordinateRepository> _coordinateRepositoryMock;
    private readonly UpdateCoordinateHandler _handler;

    public UpdateCoordinateHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();
        _coordinateRepositoryMock = new Mock<IStreetcodeCoordinateRepository>();

        _repositoryWrapperMock
            .Setup(r => r.StreetcodeCoordinateRepository)
            .Returns(_coordinateRepositoryMock.Object);

        _handler = new UpdateCoordinateHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenMapperReturnsNull()
    {
        var command = new UpdateCoordinateCommand(null!);

        _mapperMock
            .Setup(m => m.Map<StreetcodeCoordinate>(
                command.StreetcodeCoordinate))
            .Returns((StreetcodeCoordinate)null!);

        var result = await _handler.Handle(command, CancellationToken.None);

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

        _mapperMock
            .Setup(m => m.Map<StreetcodeCoordinate>(coordinateDto))
            .Returns(coordinate);

        _repositoryWrapperMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should()
            .Be(ErrorMessages.FailedToUpdateStreetcodeCoordinate);

        _coordinateRepositoryMock.Verify(
            r => r.Update(coordinate),
            Times.Once);

        _repositoryWrapperMock.Verify(
            r => r.SaveChangesAsync(),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCoordinateUpdated()
    {
        var coordinateDto = new StreetcodeCoordinateDTO();
        var command = new UpdateCoordinateCommand(coordinateDto);
        var coordinate = new StreetcodeCoordinate();

        _mapperMock
            .Setup(m => m.Map<StreetcodeCoordinate>(coordinateDto))
            .Returns(coordinate);

        _repositoryWrapperMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _coordinateRepositoryMock.Verify(
            r => r.Update(coordinate),
            Times.Once);

        _repositoryWrapperMock.Verify(
            r => r.SaveChangesAsync(),
            Times.Once);
    }
}