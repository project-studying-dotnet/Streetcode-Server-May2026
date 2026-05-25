using AutoMapper;
using FluentAssertions;
using FluentResults;
using Moq;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Update;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

namespace Streetcode.XUnitTest.BLL.MediatR.AdditionalContent.Coordinate;

public class UpdateCoordinateHandlerTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;

    private readonly UpdateCoordinateHandler _handler;

    public UpdateCoordinateHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();

        _handler = new UpdateCoordinateHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenMapperReturnsNull()
    {
        // Arrange
        var command = new UpdateCoordinateCommand(null);

        _mapperMock
            .Setup(m => m.Map<DAL.Entities.AdditionalContent.Coordinates.Types.StreetcodeCoordinate>(
                It.IsAny<object>()))
            .Returns((DAL.Entities.AdditionalContent.Coordinates.Types.StreetcodeCoordinate)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();

        result.Errors[0].Message.Should()
            .Be(ErrorMessages.CannotConvertNullToStreetcodeCoordinate);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenUpdateFails()
    {
        // Arrange
        var coordinate =
            new DAL.Entities.AdditionalContent.Coordinates.Types.StreetcodeCoordinate();

        var command = new UpdateCoordinateCommand(
            new Streetcode.BLL.DTO.AdditionalContent.Coordinates.StreetcodeCoordinateDTO());

        _mapperMock
            .Setup(m => m.Map<DAL.Entities.AdditionalContent.Coordinates.Types.StreetcodeCoordinate>(
                It.IsAny<object>()))
            .Returns(coordinate);

        _repositoryWrapperMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();

        result.Errors[0].Message.Should()
            .Be(ErrorMessages.FailedToUpdateStreetcodeCoordinate);

        _repositoryWrapperMock.Verify(
            r => r.StreetcodeCoordinateRepository.Update(coordinate),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCoordinateUpdated()
    {
        // Arrange
        var coordinate =
            new DAL.Entities.AdditionalContent.Coordinates.Types.StreetcodeCoordinate();

        var command = new UpdateCoordinateCommand(
            new Streetcode.BLL.DTO.AdditionalContent.Coordinates.StreetcodeCoordinateDTO());

        _mapperMock
            .Setup(m => m.Map<DAL.Entities.AdditionalContent.Coordinates.Types.StreetcodeCoordinate>(
                It.IsAny<object>()))
            .Returns(coordinate);

        _repositoryWrapperMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _repositoryWrapperMock.Verify(
            r => r.StreetcodeCoordinateRepository.Update(coordinate),
            Times.Once);

        _repositoryWrapperMock.Verify(
            r => r.SaveChangesAsync(),
            Times.Once);
    }
}