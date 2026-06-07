using System.Linq.Expressions;
using Moq;
using Xunit;
using AutoMapper;
using FluentAssertions;
using MockQueryable.Moq;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Update;
using Streetcode.BLL.Resources;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.MediatR.Timeline.TimelineItem;
using Streetcode.DAL.Repositories.Interfaces.Timeline;
using HistContext = Streetcode.DAL.Entities.Timeline.HistoricalContext;
using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.BLL.MediatR.Timeline.TimelineItem;

public sealed class CreateTimelineItemHandlerTests
{
    private Mock<IRepositoryWrapper> RepositoryWrapperMock { get; }
    private Mock<ITimelineRepository> TimelineRepositoryMock { get; }
    private Mock<IHistoricalContextTimelineRepository> HistoricalContextTimelineRepositoryMock { get; }
    private Mock<ILoggerService> LoggerMock { get; }
    private IMapper Mapper { get; }
    private CreateTimelineItemHandler Handler { get; }

    public CreateTimelineItemHandlerTests()
    {
        RepositoryWrapperMock = new Mock<IRepositoryWrapper>();
        TimelineRepositoryMock = new Mock<ITimelineRepository>();
        HistoricalContextTimelineRepositoryMock = new Mock<IHistoricalContextTimelineRepository>();
        RepositoryWrapperMock.Setup(r => r.TimelineRepository).Returns(TimelineRepositoryMock.Object);
        RepositoryWrapperMock.Setup(r => r.HistoricalContextTimelineRepository).Returns(HistoricalContextTimelineRepositoryMock.Object);

        LoggerMock = new Mock<ILoggerService>();
        Mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(CreateTimelineItemHandler).Assembly);
        }).CreateMapper();
        Handler = new CreateTimelineItemHandler(RepositoryWrapperMock.Object, Mapper, LoggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsTimelineItem_WhenCreatedSuccessfully()
    {
        // Arrange
        List<HistoricalContextTimeline> hcts = [
            new HistoricalContextTimeline()
            {
                HistoricalContext = new HistContext()
                {
                    Id = 0,
                    Title = "Title"
                },
                HistoricalContextId = 2,
                Timeline = new TimelineItemEntity()
                {
                    Id = 1,
                    Title = "Title"
                },
                TimelineId = 0
            }
        ];
        TimelineItemEntity timeline_item = new()
        {
            Id = 0,
            Title = "Test Timeline Item",
            HistoricalContextTimelines = hcts
        };
        TimelineItemDto dto = Mapper.Map<TimelineItemDto>(timeline_item);
        CreateTimelineItemCommand command = new(dto);
        TimelineRepositoryMock.Setup(
            r => r.CreateAsync(It.IsAny<TimelineItemEntity>())
        ).ReturnsAsync(static (TimelineItemEntity ti) =>
        {
            ti.Id = 1; // Simulate database assigning an ID
            return ti;
        });
        HistoricalContextTimelineRepositoryMock.Setup(
            r => r.CreateRangeAsync(It.IsAny<IEnumerable<HistoricalContextTimeline>>())
        );
        RepositoryWrapperMock.Setup(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(1);
        HistoricalContextTimelineRepositoryMock.Setup(
            r => r.FindAll(
                It.IsAny<Expression<Func<HistoricalContextTimeline, bool>>>()
            )
        ).Returns(hcts.BuildMock());

        // Act
        Result<TimelineItemDto> result = await Handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(dto, opt => opt.Excluding(dto => dto.Id));
        result.Value.Id.Should().Be(1);
        TimelineRepositoryMock.Verify(
            r => r.CreateAsync(It.IsAny<TimelineItemEntity>()),
            Times.Once
        );
        HistoricalContextTimelineRepositoryMock.Verify(
            r => r.CreateRangeAsync(It.IsAny<IEnumerable<HistoricalContextTimeline>>()),
            Times.Once
        );
        RepositoryWrapperMock.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
        HistoricalContextTimelineRepositoryMock.Verify(
            r => r.FindAll(
                It.IsAny<Expression<Func<HistoricalContextTimeline, bool>>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenCreateFails()
    {
        // Arrange
        string error_msg = string.Format(ErrorMessages.FailedToCreateType, nameof(TimelineItemEntity));
        TimelineItemEntity timeline_item = new()
        {
            Id = 0,
            Title = "Test Timeline Item",
        };
        TimelineItemDto dto = Mapper.Map<TimelineItemDto>(timeline_item);
        CreateTimelineItemCommand command = new(dto);
        TimelineRepositoryMock.Setup(
            r => r.CreateAsync(It.IsAny<TimelineItemEntity>())
        ).ReturnsAsync(static (TimelineItemEntity ti) =>
        {
            ti.Id = 1; // Simulate database assigning an ID
            return ti;
        });
        RepositoryWrapperMock.Setup(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(0);
        LoggerMock.Setup(l => l.LogError(command, error_msg));

        // Act
        Result<TimelineItemDto> result = await Handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().BeEquivalentTo([
            new Error(error_msg)
        ]);
        TimelineRepositoryMock.Verify(
            r => r.CreateAsync(It.IsAny<TimelineItemEntity>()),
            Times.Once
        );
        RepositoryWrapperMock.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        LoggerMock.Verify(l => l.LogError(command, error_msg), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenHistoricalContextTimelineCreateFails()
    {
        // Arrange
        string error_msg = string.Format(ErrorMessages.FailedToCreateType, nameof(HistoricalContextTimeline));
        TimelineItemEntity timeline_item = new()
        {
            Id = 0,
            Title = "Test Timeline Item",
            HistoricalContextTimelines = [
                new HistoricalContextTimeline()
                {
                    HistoricalContext = new HistContext()
                    {
                        Id = 0,
                        Title = "Title"
                    },
                    HistoricalContextId = 2,
                    Timeline = new TimelineItemEntity()
                    {
                        Id = 1,
                        Title = "Title"
                    },
                    TimelineId = 0
                }
            ]
        };
        TimelineItemDto dto = Mapper.Map<TimelineItemDto>(timeline_item);
        CreateTimelineItemCommand command = new(dto);
        TimelineRepositoryMock.Setup(
            r => r.CreateAsync(It.IsAny<TimelineItemEntity>())
        ).ReturnsAsync(static (TimelineItemEntity ti) =>
        {
            ti.Id = 1; // Simulate database assigning an ID
            return ti;
        });
        HistoricalContextTimelineRepositoryMock.Setup(
            r => r.CreateRangeAsync(It.IsAny<IEnumerable<HistoricalContextTimeline>>())
        );
        RepositoryWrapperMock.SetupSequence(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(1).ReturnsAsync(0);
        LoggerMock.Setup(l => l.LogError(command, error_msg));

        // Act
        Result<TimelineItemDto> result = await Handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().BeEquivalentTo([
            new Error(error_msg)
        ]);
        TimelineRepositoryMock.Verify(
            r => r.CreateAsync(It.IsAny<TimelineItemEntity>()),
            Times.Once
        );
        HistoricalContextTimelineRepositoryMock.Verify(
            r => r.CreateRangeAsync(It.IsAny<IEnumerable<HistoricalContextTimeline>>()),
            Times.Once
        );
        RepositoryWrapperMock.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
        LoggerMock.Verify(l => l.LogError(command, error_msg), Times.Once);
    }
}
