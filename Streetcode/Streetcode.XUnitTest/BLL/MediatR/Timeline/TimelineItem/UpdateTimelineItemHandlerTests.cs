using System.Linq.Expressions;
using Moq;
using Xunit;
using AutoMapper;
using FluentResults;
using FluentAssertions;
using MockQueryable.Moq;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.MediatR.Timeline.TimelineItem;
using Streetcode.DAL.Repositories.Interfaces.Timeline;
using HistContext = Streetcode.DAL.Entities.Timeline.HistoricalContext;
using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.BLL.MediatR.Timeline.TimelineItem;

public sealed class UpdateTimelineItemHandlerTests
{
    private Mock<IRepositoryWrapper> RepositoryWrapperMock { get; }
    private Mock<ITimelineRepository> TimelineRepositoryMock { get; }
    private Mock<IHistoricalContextTimelineRepository> HistoricalContextTimelineRepositoryMock { get; }
    private Mock<ILoggerService> LoggerMock { get; }
    private IMapper Mapper { get; }
    private UpdateTimelineItemHandler Handler { get; }

    public UpdateTimelineItemHandlerTests()
    {
        RepositoryWrapperMock = new Mock<IRepositoryWrapper>();
        TimelineRepositoryMock = new Mock<ITimelineRepository>();
        HistoricalContextTimelineRepositoryMock = new Mock<IHistoricalContextTimelineRepository>();
        RepositoryWrapperMock.Setup(r => r.TimelineRepository).Returns(TimelineRepositoryMock.Object);
        RepositoryWrapperMock.Setup(r => r.HistoricalContextTimelineRepository).Returns(HistoricalContextTimelineRepositoryMock.Object);

        LoggerMock = new Mock<ILoggerService>();
        Mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(UpdateTimelineItemHandler).Assembly);
        }).CreateMapper();
        Handler = new UpdateTimelineItemHandler(RepositoryWrapperMock.Object, Mapper, LoggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsTimelineItem_WhenUpdatedSuccessfully()
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
            HistoricalContextTimelines = [
                new HistoricalContextTimeline()
                {
                    HistoricalContext = new HistContext()
                    {
                        Id = 0,
                        Title = "Title"
                    },
                    HistoricalContextId = 3,
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
        UpdateTimelineItemCommand command = new(dto);
        TimelineRepositoryMock.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(timeline_item);
        TimelineRepositoryMock.Setup(
            r => r.Update(It.IsAny<TimelineItemEntity>())
        );
        HistoricalContextTimelineRepositoryMock.Setup(
            r => r.DeleteRange(It.IsAny<IEnumerable<HistoricalContextTimeline>>())
        );
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
        result.Value.Should().BeEquivalentTo(dto);
        TimelineRepositoryMock.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        TimelineRepositoryMock.Verify(
            r => r.Update(It.IsAny<TimelineItemEntity>()),
            Times.Once
        );
        HistoricalContextTimelineRepositoryMock.Verify(
            r => r.DeleteRange(It.IsAny<IEnumerable<HistoricalContextTimeline>>()),
            Times.Once
        );
        HistoricalContextTimelineRepositoryMock.Verify(
            r => r.CreateRangeAsync(It.IsAny<IEnumerable<HistoricalContextTimeline>>()),
            Times.Once
        );
        RepositoryWrapperMock.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(3)
        );
        HistoricalContextTimelineRepositoryMock.Verify(
            r => r.FindAll(
                It.IsAny<Expression<Func<HistoricalContextTimeline, bool>>>()
            ),
            Times.Exactly(2)
        );
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenItemNotFound()
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
            HistoricalContextTimelines = [
                new HistoricalContextTimeline()
                {
                    HistoricalContext = new HistContext()
                    {
                        Id = 0,
                        Title = "Title"
                    },
                    HistoricalContextId = 3,
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
        string error_msg = string.Format(ErrorMessages.TypeWithIdNotFound, nameof(TimelineItemEntity), dto.Id);
        UpdateTimelineItemCommand command = new(dto);
        TimelineRepositoryMock.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync((TimelineItemEntity?)null);
        LoggerMock.Setup(l => l.LogError(command, error_msg));

        // Act
        Result<TimelineItemDto> result = await Handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().BeEquivalentTo([
            new Error(error_msg)
        ]);
        TimelineRepositoryMock.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        LoggerMock.Verify(l => l.LogError(command, error_msg), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenUpdateFails()
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
            HistoricalContextTimelines = [
                new HistoricalContextTimeline()
                {
                    HistoricalContext = new HistContext()
                    {
                        Id = 0,
                        Title = "Title"
                    },
                    HistoricalContextId = 3,
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
        string error_msg = string.Format(ErrorMessages.FailedToUpdateType, nameof(TimelineItemEntity));
        UpdateTimelineItemCommand command = new(dto);
        TimelineRepositoryMock.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(timeline_item);
        TimelineRepositoryMock.Setup(
            r => r.Update(It.IsAny<TimelineItemEntity>())
        );
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
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        TimelineRepositoryMock.Verify(
            r => r.Update(It.IsAny<TimelineItemEntity>()),
            Times.Once
        );
        RepositoryWrapperMock.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        LoggerMock.Verify(l => l.LogError(command, error_msg), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenDeleteHistoricalContextTimelineFails()
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
            HistoricalContextTimelines = [
                new HistoricalContextTimeline()
                {
                    HistoricalContext = new HistContext()
                    {
                        Id = 0,
                        Title = "Title"
                    },
                    HistoricalContextId = 3,
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
        string error_msg = string.Format(ErrorMessages.FailedToDeleteType, nameof(HistoricalContextTimeline));
        UpdateTimelineItemCommand command = new(dto);
        TimelineRepositoryMock.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(timeline_item);
        TimelineRepositoryMock.Setup(
            r => r.Update(It.IsAny<TimelineItemEntity>())
        );
        HistoricalContextTimelineRepositoryMock.Setup(
            r => r.DeleteRange(It.IsAny<IEnumerable<HistoricalContextTimeline>>())
        );
        HistoricalContextTimelineRepositoryMock.Setup(
            r => r.FindAll(
                It.IsAny<Expression<Func<HistoricalContextTimeline, bool>>>()
            )
        ).Returns(hcts.BuildMock());
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
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        TimelineRepositoryMock.Verify(
            r => r.Update(It.IsAny<TimelineItemEntity>()),
            Times.Once
        );
        HistoricalContextTimelineRepositoryMock.Verify(
            r => r.DeleteRange(It.IsAny<IEnumerable<HistoricalContextTimeline>>()),
            Times.Once
        );
        HistoricalContextTimelineRepositoryMock.Verify(
            r => r.FindAll(
                It.IsAny<Expression<Func<HistoricalContextTimeline, bool>>>()
            ),
            Times.Once
        );
        RepositoryWrapperMock.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
        LoggerMock.Verify(l => l.LogError(command, error_msg), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenCreateHistoricalContextTimelineFails()
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
            HistoricalContextTimelines = [
                new HistoricalContextTimeline()
                {
                    HistoricalContext = new HistContext()
                    {
                        Id = 0,
                        Title = "Title"
                    },
                    HistoricalContextId = 3,
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
        string error_msg = string.Format(ErrorMessages.FailedToCreateType, nameof(HistoricalContextTimeline));
        UpdateTimelineItemCommand command = new(dto);
        TimelineRepositoryMock.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(timeline_item);
        TimelineRepositoryMock.Setup(
            r => r.Update(It.IsAny<TimelineItemEntity>())
        );
        HistoricalContextTimelineRepositoryMock.Setup(
            r => r.DeleteRange(It.IsAny<IEnumerable<HistoricalContextTimeline>>())
        );
        HistoricalContextTimelineRepositoryMock.Setup(
            r => r.CreateRangeAsync(It.IsAny<IEnumerable<HistoricalContextTimeline>>())
        );
        HistoricalContextTimelineRepositoryMock.Setup(
            r => r.FindAll(
                It.IsAny<Expression<Func<HistoricalContextTimeline, bool>>>()
            )
        ).Returns(hcts.BuildMock());
        RepositoryWrapperMock.SetupSequence(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(1).ReturnsAsync(1).ReturnsAsync(0);
        LoggerMock.Setup(l => l.LogError(command, error_msg));

        // Act
        Result<TimelineItemDto> result = await Handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().BeEquivalentTo([
            new Error(error_msg)
        ]);
        TimelineRepositoryMock.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        TimelineRepositoryMock.Verify(
            r => r.Update(It.IsAny<TimelineItemEntity>()),
            Times.Once
        );
        HistoricalContextTimelineRepositoryMock.Verify(
            r => r.DeleteRange(It.IsAny<IEnumerable<HistoricalContextTimeline>>()),
            Times.Once
        );
        HistoricalContextTimelineRepositoryMock.Verify(
            r => r.CreateRangeAsync(It.IsAny<IEnumerable<HistoricalContextTimeline>>()),
            Times.Once
        );
        HistoricalContextTimelineRepositoryMock.Verify(
            r => r.FindAll(
                It.IsAny<Expression<Func<HistoricalContextTimeline, bool>>>()
            ),
            Times.Once
        );
        RepositoryWrapperMock.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(3)
        );
        LoggerMock.Verify(l => l.LogError(command, error_msg), Times.Once);
    }
}