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
        this.mapperMock = new Mock<IMapper>();
        this.repositoryWrapperMock = new Mock<IRepositoryWrapper>();

        this.handler = new CreateCoordinateHandler(
            this.repositoryWrapperMock.Object,
            this.mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenMapperReturnsNull()
    {
        var command = new CreateCoordinateCommand(null!);

        this.mapperMock
            .Setup(m => m.Map<DAL.Entities.AdditionalContent.Coordinates.Types.StreetcodeCoordinate>(
                command.StreetcodeCoordinate))
            .Returns((DAL.Entities.AdditionalContent.Coordinates.Types.StreetcodeCoordinate?)null);

        var result = await this.handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.CannotConvertNullToStreetcodeCoordinate);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenSaveChangesFails()
    {
        var command = new CreateCoordinateCommand(null!);

        var coordinate =
            new DAL.Entities.AdditionalContent.Coordinates.Types.StreetcodeCoordinate();

        this.mapperMock
            .Setup(m => m.Map<DAL.Entities.AdditionalContent.Coordinates.Types.StreetcodeCoordinate>(
                command.StreetcodeCoordinate))
            .Returns(coordinate);

        this.repositoryWrapperMock
            .Setup(r => r.StreetcodeCoordinateRepository.Create(coordinate));

        this.repositoryWrapperMock
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(0);

        var result = await this.handler.Handle(command, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Be(ErrorMessages.FailedToCreateStreetcodeCoordinate);
    }
}