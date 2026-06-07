using AutoMapper;
using FluentAssertions;
using FluentResults;
using Moq;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext.Create;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Timeline;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext;
using HistContext = Streetcode.DAL.Entities.Timeline.HistoricalContext;

namespace Streetcode.XUnitTest.BLL.MediatR.Timeline.HistoricalContext;

public sealed class CreateHistoricalContextHandlerTests
{
    private Mock<IRepositoryWrapper> RepositoryWrapperMock { get; }
    private Mock<IHistoricalContextRepository> HistoricalContextRepositoryMock { get; }
    private Mock<ILoggerService> LoggerMock { get; }
    private IMapper Mapper { get; }
    private CreateHistoricalContextHandler Handler { get; }

    public CreateHistoricalContextHandlerTests()
    {
        RepositoryWrapperMock = new Mock<IRepositoryWrapper>();
        HistoricalContextRepositoryMock = new Mock<IHistoricalContextRepository>();
        RepositoryWrapperMock.Setup(r => r.HistoricalContextRepository).Returns(HistoricalContextRepositoryMock.Object);

        LoggerMock = new Mock<ILoggerService>();
        MapperConfiguration config = new(cfg =>
        {
            cfg.AddMaps(typeof(CreateHistoricalContextHandler).Assembly);
        });
        Mapper = config.CreateMapper();
        Handler = new CreateHistoricalContextHandler(RepositoryWrapperMock.Object, Mapper, LoggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsHistoricalContext_WhenCreatedSuccessfully()
    {
        // Arrange
        HistoricalContextDto dto = new()
        {
            Title = "Test Historical Context"
        };
        CreateHistoricalContextCommand command = new(dto);
        HistoricalContextRepositoryMock.Setup(
            r => r.CreateAsync(It.IsAny<HistContext>())
        ).ReturnsAsync(static(HistContext hc) =>
        {
            hc.Id = 1; // Simulate database assigning an ID
            return hc;
        });
        RepositoryWrapperMock.Setup(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(1);

        // Act
        Result<HistoricalContextDto> result = await Handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        dto.Should().BeEquivalentTo(result.Value, options => options.Excluding(x => x.Id));
        result.Value.Id.Should().Be(1);
        HistoricalContextRepositoryMock.Verify(
            r => r.CreateAsync(It.IsAny<HistContext>()),
            Times.Once
        );
        RepositoryWrapperMock.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenCreateFails()
    {
        // Arrange
        string error_msg = string.Format(ErrorMessages.FailedToCreateType, nameof(HistContext));
        HistoricalContextDto dto = new()
        {
            Title = "Test Historical Context"
        };
        CreateHistoricalContextCommand command = new(dto);
        HistoricalContextRepositoryMock.Setup(
            r => r.CreateAsync(It.IsAny<HistContext>())
        ).ReturnsAsync(static(HistContext hc) =>
        {
            hc.Id = 1; // Simulate database assigning an ID
            return hc;
        });
        RepositoryWrapperMock.Setup(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(0);
        LoggerMock.Setup(l => l.LogError(command, error_msg));

        // Act
        Result<HistoricalContextDto> result = await Handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo([
            new Error(error_msg)
        ]);
        HistoricalContextRepositoryMock.Verify(
            r => r.CreateAsync(It.IsAny<HistContext>()),
            Times.Once
        );
        RepositoryWrapperMock.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        LoggerMock.Verify(l => l.LogError(command, error_msg), Times.Once);
    }
}
