using AutoMapper;
using FluentAssertions;
using Moq;
using Streetcode.BLL.MediatR.AdditionalContent.Coordinate.Create;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;

using StreetcodeCoordinateEntity = global::Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types.StreetcodeCoordinate;

namespace Streetcode.XUnitTest.BLL.MediatR.AdditionalContent.Coordinate.Create;

public class CreateCoordinateHandlerTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IRepositoryWrapper> _repositoryWrapperMock;
    private readonly CreateCoordinateHandler _handler;

    public CreateCoordinateHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _repositoryWrapperMock = new Mock<IRepositoryWrapper>();

        _handler = new CreateCoordinateHandler(
            _repositoryWrapperMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenMapperReturnsNull()
    {
        var command = new CreateCoordinateCommand(null!);

        _mapperMock
            .Setup(m => m.Map<StreetcodeCoordinateEntity>(
                command.StreetcodeCoordinate))
            .Returns((StreetcodeCoordinateEntity)null!);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.CannotConvertNullToStreetcodeCoordinate);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
    {
        var command = new CreateCoordinateCommand(null!);

        var coordinate =
            new StreetcodeCoordinateEntity();

        _mapperMock
            .Setup(m => m.Map<StreetcodeCoordinateEntity>(
                command.StreetcodeCoordinate))
            .Returns(coordinate);

        _repositoryWrapperMock
            .Setup(r => r.StreetcodeCoordinateRepository.Create(coordinate));

        _repositoryWrapperMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.FailedToCreateStreetcodeCoordinate);
    }
}