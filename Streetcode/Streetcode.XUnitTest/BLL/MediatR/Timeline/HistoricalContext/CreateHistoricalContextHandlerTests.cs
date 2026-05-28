using Moq;
using Xunit;
using AutoMapper;
using FluentResults;
using FluentAssertions;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Timeline;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext.Create;
using HistContext = Streetcode.DAL.Entities.Timeline.HistoricalContext;

namespace Streetcode.XUnitTest.BLL.MediatR.Timeline.HistoricalContext;

public sealed class CreateHistoricalContextHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<IHistoricalContextRepository> _mockHistoricalContextRepository;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly IMapper _mapper;
    private readonly CreateHistoricalContextHandler _handler;

    public CreateHistoricalContextHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockHistoricalContextRepository = new Mock<IHistoricalContextRepository>();
        _mockRepositoryWrapper.Setup(r => r.HistoricalContextRepository).Returns(_mockHistoricalContextRepository.Object);

        _mockLogger = new Mock<ILoggerService>();
        MapperConfiguration config = new(cfg =>
        {
            cfg.AddMaps(typeof(CreateHistoricalContextHandler).Assembly);
        });
        _mapper = config.CreateMapper();
        _handler = new CreateHistoricalContextHandler(_mockRepositoryWrapper.Object, _mapper, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ReturnsCreatedHistoricalContext_WhenCreatedSuccessfully()
    {
        // Arrange
        HistoricalContextDto dto = new()
        {
            Title = "Test Historical Context"
        };
        CreateHistoricalContextCommand command = new(dto);
        _mockHistoricalContextRepository.Setup(
            r => r.CreateAsync(It.IsAny<HistContext>())
        ).ReturnsAsync(static (HistContext hc) =>
        {
            hc.Id = 1; // Simulate database assigning an ID
            return hc;
        });
        _mockRepositoryWrapper.Setup(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(1);

        // Act
        Result<HistoricalContextDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        dto.Should().BeEquivalentTo(result.Value, options => options.Excluding(x => x.Id));
        result.Value.Id.Should().Be(1);
        _mockHistoricalContextRepository.Verify(
            r => r.CreateAsync(It.IsAny<HistContext>()),
            Times.Once
        );
        _mockRepositoryWrapper.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenCreateFails()
    {
        // Arrange
        HistoricalContextDto dto = new()
        {
            Title = "Test Historical Context"
        };
        CreateHistoricalContextCommand command = new(dto);
        _mockHistoricalContextRepository.Setup(
            r => r.CreateAsync(It.IsAny<HistContext>())
        ).ReturnsAsync(static (HistContext hc) =>
        {
            hc.Id = 1; // Simulate database assigning an ID
            return hc;
        });
        _mockRepositoryWrapper.Setup(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(0);
        _mockLogger.Setup(
            l => l.LogError(command, ErrorMessages.CannotSaveHistoricalContextToDatabase)
        );

        // Act
        Result<HistoricalContextDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo([new Error(ErrorMessages.CannotSaveHistoricalContextToDatabase)]);
        _mockHistoricalContextRepository.Verify(
            r => r.CreateAsync(It.IsAny<HistContext>()),
            Times.Once
        );
        _mockRepositoryWrapper.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        _mockLogger.Verify(
            l => l.LogError(command, ErrorMessages.CannotSaveHistoricalContextToDatabase),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenExceptionOccurs()
    {
        // Arrange
        const string exception_message = "Database error";
        HistoricalContextDto dto = new()
        {
            Title = "Test Historical Context"
        };
        CreateHistoricalContextCommand command = new(dto);
        _mockHistoricalContextRepository.Setup(
            r => r.CreateAsync(It.IsAny<HistContext>())
        ).ReturnsAsync(static (HistContext hc) =>
        {
            hc.Id = 1; // Simulate database assigning an ID
            return hc;
        });
        _mockRepositoryWrapper.Setup(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ThrowsAsync(new Exception(exception_message));
        _mockLogger.Setup(
            l => l.LogError(command, ErrorMessages.CannotSaveHistoricalContextToDatabase)
        );

        // Act
        Result<HistoricalContextDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo([new Error(exception_message)]);
        _mockHistoricalContextRepository.Verify(
            r => r.CreateAsync(It.IsAny<HistContext>()),
            Times.Once
        );
        _mockRepositoryWrapper.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        _mockLogger.Verify(
            l => l.LogError(command, exception_message),
            Times.Once
        );
    }
}