using AutoMapper;
using FluentAssertions;
using FluentResults;
using MediatR;
using Moq;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Update;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Timeline;
using Xunit;

using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.BLL.MediatR.Timeline.TimelineItem;

public sealed class CreateTimelineItemHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ITimelineRepository> _mockTimelineRepository;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly Mock<IMediator> _mockMediator;
    private readonly IMapper _mapper;
    private readonly CreateTimelineItemHandler _handler;

    public CreateTimelineItemHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockTimelineRepository = new Mock<ITimelineRepository>();
        _mockRepositoryWrapper.Setup(r => r.TimelineRepository).Returns(_mockTimelineRepository.Object);

        _mockLogger = new Mock<ILoggerService>();
        _mockMediator = new Mock<IMediator>();
        MapperConfiguration config = new(cfg =>
        {
            cfg.AddMaps(typeof(CreateTimelineItemHandler).Assembly);
        });
        _mapper = config.CreateMapper();
        _handler = new CreateTimelineItemHandler(_mockRepositoryWrapper.Object, _mapper, _mockMediator.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ReturnsCreatedTimelineItem_WhenCreatedSuccessfully()
    {
        // Arrange
        TimelineItemDto dto = new()
        {
            Id = 0,
            Title = "Test Timeline Item",
            Description = "Description of the test timeline item.",
            Date = new DateTime(2024, 1, 1),
            DateViewPattern = DateViewPattern.DateMonthYear,
            StreetcodeId = 1,
            HistoricalContexts = [
                new HistoricalContextDto()
                {
                    Id = 1,
                    Title = "Context 1",
                }
            ]
        };
        CreateTimelineItemCommand command = new(dto);

        // This makes handler return the dto, regardless of the input.
        // So we will only verify calls of the mocked methods
        _mockMediator.Setup(
            m => m.Send(It.IsAny<UpdateTimelineItemCommand>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(Result.Ok(dto));
        _mockTimelineRepository.Setup(
            r => r.CreateAsync(It.IsAny<TimelineItemEntity>())
        ).ReturnsAsync(static (TimelineItemEntity ti) =>
        {
            ti.Id = 1; // Simulate database assigning an ID
            return ti;
        });
        _mockRepositoryWrapper.Setup(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(1);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockTimelineRepository.Verify(
            r => r.CreateAsync(It.IsAny<TimelineItemEntity>()),
            Times.Once
        );
        _mockRepositoryWrapper.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.AtLeastOnce
        );
        _mockMediator.Verify(
            m => m.Send(It.IsAny<UpdateTimelineItemCommand>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenCreateFails()
    {
        // Arrange
        TimelineItemDto dto = new()
        {
            Id = 0,
            Title = "Test Timeline Item",
            Description = "Description of the test timeline item.",
            Date = new DateTime(2024, 1, 1),
            DateViewPattern = DateViewPattern.DateMonthYear,
            StreetcodeId = 1,
            HistoricalContexts = [
                new HistoricalContextDto()
                {
                    Id = 1,
                    Title = "Context 1",
                }
            ]
        };
        CreateTimelineItemCommand command = new(dto);
        _mockTimelineRepository.Setup(
            r => r.CreateAsync(It.IsAny<TimelineItemEntity>())
        ).ReturnsAsync(static (TimelineItemEntity ti) =>
        {
            ti.Id = 1; // Simulate database assigning an ID
            return ti;
        });
        _mockRepositoryWrapper.Setup(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(0);
        _mockLogger.Setup(
            l => l.LogError(command, ErrorMessages.CannotSaveTimelineItem)
        );

        // Act
        Result<TimelineItemDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo([new Error(ErrorMessages.CannotSaveTimelineItem)]);
        _mockTimelineRepository.Verify(
            r => r.CreateAsync(It.IsAny<TimelineItemEntity>()),
            Times.Once
        );
        _mockRepositoryWrapper.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        _mockLogger.Verify(
            l => l.LogError(command, ErrorMessages.CannotSaveTimelineItem),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenExceptionOccurs()
    {
        // Arrange
        const string exception_message = "Database exception";
        TimelineItemDto dto = new()
        {
            Id = 0,
            Title = "Test Timeline Item",
            Description = "Description of the test timeline item.",
            Date = new DateTime(2024, 1, 1),
            DateViewPattern = DateViewPattern.DateMonthYear,
            StreetcodeId = 1,
            HistoricalContexts = [
                new HistoricalContextDto()
                {
                    Id = 1,
                    Title = "Context 1",
                }
            ]
        };
        CreateTimelineItemCommand command = new(dto);
        _mockTimelineRepository.Setup(
            r => r.CreateAsync(It.IsAny<TimelineItemEntity>())
        ).ReturnsAsync(static (TimelineItemEntity ti) =>
        {
            ti.Id = 1; // Simulate database assigning an ID
            return ti;
        });
        _mockRepositoryWrapper.Setup(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ThrowsAsync(new Exception(exception_message));
        _mockLogger.Setup(
            l => l.LogError(command, exception_message)
        );

        // Act
        Result<TimelineItemDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo([new Error(exception_message)]);
        _mockTimelineRepository.Verify(
            r => r.CreateAsync(It.IsAny<TimelineItemEntity>()),
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
