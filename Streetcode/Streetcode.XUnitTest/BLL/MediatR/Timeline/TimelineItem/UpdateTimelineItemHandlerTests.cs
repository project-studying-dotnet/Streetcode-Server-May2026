using System.Linq.Expressions;
using Moq;
using Xunit;
using AutoMapper;
using FluentResults;
using FluentAssertions;
using Streetcode.DAL.Enums;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Timeline;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Update;
using HistContext = Streetcode.DAL.Entities.Timeline.HistoricalContext;
using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.BLL.MediatR.Timeline.TimelineItem;

public sealed class UpdateTimelineItemHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ITimelineRepository> _mockTimelineRepository;
    private readonly Mock<IHistoricalContextRepository> _mockHistoricalContextRepository;
    private readonly Mock<IHistoricalContextTimelineRepository> _mockHistoricalContextTimelineRepository;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly IMapper _mapper;
    private readonly UpdateTimelineItemHandler _handler;

    public UpdateTimelineItemHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockTimelineRepository = new Mock<ITimelineRepository>();
        _mockHistoricalContextRepository = new Mock<IHistoricalContextRepository>();
        _mockHistoricalContextTimelineRepository = new Mock<IHistoricalContextTimelineRepository>();

        _mockRepositoryWrapper.Setup(r => r.TimelineRepository).Returns(_mockTimelineRepository.Object);
        _mockRepositoryWrapper.Setup(r => r.HistoricalContextRepository).Returns(_mockHistoricalContextRepository.Object);
        _mockRepositoryWrapper.Setup(r => r.HistoricalContextTimelineRepository).Returns(_mockHistoricalContextTimelineRepository.Object);

        _mockLogger = new Mock<ILoggerService>();
        MapperConfiguration config = new(cfg =>
        {
            cfg.AddMaps(typeof(UpdateTimelineItemHandler).Assembly);
        });
        _mapper = config.CreateMapper();
        _handler = new UpdateTimelineItemHandler(_mockRepositoryWrapper.Object, _mapper, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ReturnsUpdatedTimelineItem_WhenUpdatedSuccessfully()
    {
        // Arrange
        TimelineItemDto dto = new()
        {
            Id = 1,
            Title = "Updated Timeline Item",
            Description = "Updated description of the timeline item.",
            Date = new DateTime(2024, 1, 1),
            DateViewPattern = DateViewPattern.DateMonthYear,
            StreetcodeId = 1,
            HistoricalContexts = [
                new HistoricalContextDto()
                { 
                    Id = 1, 
                    Title = "Context 1"
                }
            ]
        };
        TimelineItemEntity timeline_item_entity = _mapper.Map<TimelineItemEntity>(dto);
        timeline_item_entity.HistoricalContextTimelines = [];
        HistContext historical_context = new()
        {
            Id = 1,
            Title = "Context 1"
        };
        UpdateTimelineItemCommand command = new(dto);

        _mockTimelineRepository.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(timeline_item_entity);
        _mockHistoricalContextTimelineRepository.Setup(
            r => r.DeleteRange(It.IsAny<IEnumerable<HistoricalContextTimeline>>())
        );
        _mockRepositoryWrapper.Setup(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(1);
        _mockHistoricalContextRepository.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<HistContext, bool>>>(),
                It.IsAny<Func<IQueryable<HistContext>, IIncludableQueryable<HistContext, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(historical_context);
        _mockTimelineRepository.Setup(
            r => r.Update(timeline_item_entity)
        );

        // Act
        Result<TimelineItemDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(dto, options => options.Excluding(t => t.HistoricalContexts));
        _mockTimelineRepository.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        _mockHistoricalContextTimelineRepository.Verify(
            r => r.DeleteRange(It.IsAny<IEnumerable<HistoricalContextTimeline>>()),
            Times.Once
        );
        _mockHistoricalContextRepository.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<HistContext, bool>>>(),
                It.IsAny<Func<IQueryable<HistContext>, IIncludableQueryable<HistContext, object>>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        _mockTimelineRepository.Verify(
            r => r.Update(It.IsAny<TimelineItemEntity>()),
            Times.Once
        );
        _mockRepositoryWrapper.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenTimelineItemNotFound()
    {
        // Arrange
        TimelineItemDto dto = new()
        {
            Id = 999,
            Title = "Test Timeline Item",
            Description = "Description of the test timeline item.",
            Date = new DateTime(2024, 1, 1),
            DateViewPattern = DateViewPattern.DateMonthYear,
            StreetcodeId = 1,
            HistoricalContexts = []
        };
        UpdateTimelineItemCommand command = new(dto);

        _mockTimelineRepository.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync((TimelineItemEntity?)null);
        _mockLogger.Setup(
            l => l.LogError(command, string.Format(ErrorMessages.TimelineItemWithIdNotFound, dto.Id))
        );

        // Act
        Result<TimelineItemDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo([
            new Error(string.Format(ErrorMessages.TimelineItemWithIdNotFound, dto.Id))
        ]);

        _mockTimelineRepository.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        _mockLogger.Verify(
            l => l.LogError(command, string.Format(ErrorMessages.TimelineItemWithIdNotFound, dto.Id)),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenHistoricalContextNotFound()
    {
        // Arrange
        TimelineItemDto dto = new()
        {
            Id = 1,
            Title = "Test Timeline Item",
            Description = "Description of the test timeline item.",
            Date = new DateTime(2024, 1, 1),
            DateViewPattern = DateViewPattern.DateMonthYear,
            StreetcodeId = 1,
            HistoricalContexts = [
                new HistoricalContextDto()
                {
                    Id = 999,
                    Title = "Non-existent Context"
                }
            ]
        };
        TimelineItemEntity timeline_item_entity = new()
        {
            Id = 1,
            Title = "Test Timeline Item",
            Description = "Description of the test timeline item.",
            Date = new DateTime(2024, 1, 1),
            DateViewPattern = DateViewPattern.DateMonthYear,
            StreetcodeId = 1,
            HistoricalContextTimelines = []
        };
        UpdateTimelineItemCommand command = new(dto);

        _mockTimelineRepository.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(timeline_item_entity);
        _mockHistoricalContextTimelineRepository.Setup(
            r => r.DeleteRange(It.IsAny<IEnumerable<HistoricalContextTimeline>>())
        );
        _mockRepositoryWrapper.Setup(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(1);
        _mockHistoricalContextRepository.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<HistContext, bool>>>(),
                It.IsAny<Func<IQueryable<HistContext>, IIncludableQueryable<HistContext, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync((HistContext?)null);
        _mockLogger.Setup(
            l => l.LogError(command, string.Format(ErrorMessages.HistoricalContextWithIdNotFound, 999))
        );

        // Act
        Result<TimelineItemDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().ContainSingle().Which.Message.Should().Be(ErrorMessages.CannotFindOneOrMoreHistoricalContexts);
        _mockTimelineRepository.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        _mockHistoricalContextRepository.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<HistContext, bool>>>(),
                It.IsAny<Func<IQueryable<HistContext>, IIncludableQueryable<HistContext, object>>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        _mockLogger.Verify(
            l => l.LogError(command, string.Format(ErrorMessages.HistoricalContextWithIdNotFound, 999)),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenUpdateFails()
    {
        // Arrange
        TimelineItemDto dto = new()
        {
            Id = 1,
            Title = "Updated Timeline Item",
            Description = "Updated description.",
            Date = new DateTime(2024, 1, 1),
            DateViewPattern = DateViewPattern.DateMonthYear,
            StreetcodeId = 1,
            HistoricalContexts = [
                new HistoricalContextDto()
                { 
                    Id = 1, 
                    Title = "Context 1"
                }
            ]
        };
        TimelineItemEntity timeline_item_entity = new()
        {
            Id = 1,
            Title = "Original Title",
            Description = "Original description.",
            Date = new DateTime(2024, 1, 1),
            DateViewPattern = DateViewPattern.DateMonthYear,
            StreetcodeId = 1,
            HistoricalContextTimelines = []
        };
        HistContext historical_context = new()
        {
            Id = 1,
            Title = "Context 1"
        };
        UpdateTimelineItemCommand command = new(dto);

        _mockTimelineRepository.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(timeline_item_entity);
        _mockHistoricalContextTimelineRepository.Setup(
            r => r.DeleteRange(It.IsAny<IEnumerable<HistoricalContextTimeline>>())
        );
        _mockRepositoryWrapper.SetupSequence(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(1).ReturnsAsync(0);
        _mockHistoricalContextRepository.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<HistContext, bool>>>(),
                It.IsAny<Func<IQueryable<HistContext>, IIncludableQueryable<HistContext, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(historical_context);
        _mockTimelineRepository.Setup(
            r => r.Update(timeline_item_entity)
        );
        _mockLogger.Setup(
            l => l.LogError(command, string.Format(ErrorMessages.FailedToUpdateTimelineItemWithId, dto.Id))
        );

        // Act
        Result<TimelineItemDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo([
            new Error(string.Format(ErrorMessages.FailedToUpdateTimelineItemWithId, dto.Id))
        ]);
        _mockTimelineRepository.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        _mockTimelineRepository.Verify(
            r => r.Update(It.IsAny<TimelineItemEntity>()),
            Times.Once
        );
        _mockRepositoryWrapper.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
        _mockLogger.Verify(
            l => l.LogError(command, string.Format(ErrorMessages.FailedToUpdateTimelineItemWithId, dto.Id)),
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
            Id = 1,
            Title = "Updated Timeline Item",
            Description = "Updated description.",
            Date = new DateTime(2024, 1, 1),
            DateViewPattern = DateViewPattern.DateMonthYear,
            StreetcodeId = 1,
            HistoricalContexts = [
                new HistoricalContextDto()
                {
                    Id = 1,
                    Title = "Context 1"
                }
            ]
        };
        TimelineItemEntity timeline_item_entity = new()
        {
            Id = 1,
            Title = "Original Title",
            Description = "Original description.",
            Date = new DateTime(2024, 1, 1),
            DateViewPattern = DateViewPattern.DateMonthYear,
            StreetcodeId = 1,
            HistoricalContextTimelines = []
        };
        HistContext historical_context = new()
        {
            Id = 1,
            Title = "Context 1"
        };
        UpdateTimelineItemCommand command = new(dto);

        _mockTimelineRepository.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(timeline_item_entity);
        _mockHistoricalContextTimelineRepository.Setup(
            r => r.DeleteRange(It.IsAny<IEnumerable<HistoricalContextTimeline>>())
        );
        _mockRepositoryWrapper.SetupSequence(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(1).ThrowsAsync(new Exception(exception_message));
        _mockHistoricalContextRepository.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<HistContext, bool>>>(),
                It.IsAny<Func<IQueryable<HistContext>, IIncludableQueryable<HistContext, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(historical_context);
        _mockTimelineRepository.Setup(
            r => r.Update(timeline_item_entity)
        );
        _mockLogger.Setup(
            l => l.LogError(command, exception_message)
        );

        // Act
        Result<TimelineItemDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo([new Error(exception_message)]);
        _mockTimelineRepository.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        _mockRepositoryWrapper.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
        _mockLogger.Verify(
            l => l.LogError(command, exception_message),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ReturnsSuccess_WhenMultipleHistoricalContextsAdded()
    {
        // Arrange
        TimelineItemDto dto = new()
        {
            Id = 1,
            Title = "Updated Timeline Item",
            Description = "Updated description with multiple contexts.",
            Date = new DateTime(2024, 1, 1),
            DateViewPattern = DateViewPattern.DateMonthYear,
            StreetcodeId = 1,
            HistoricalContexts = [
                new HistoricalContextDto()
                {
                    Id = 1,
                    Title = "Context 1"
                },
                new HistoricalContextDto()
                {
                    Id = 2,
                    Title = "Context 2"
                },
                new HistoricalContextDto()
                {
                    Id = 3,
                    Title = "Context 3"
                }
            ]
        };
        TimelineItemEntity timeline_item_entity = new()
        {
            Id = 1,
            Title = "Original Title",
            Description = "Original description.",
            Date = new DateTime(2024, 1, 1),
            DateViewPattern = DateViewPattern.DateMonthYear,
            StreetcodeId = 1,
            HistoricalContextTimelines = []
        };
        UpdateTimelineItemCommand command = new(dto);

        _mockTimelineRepository.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(timeline_item_entity);
        _mockHistoricalContextTimelineRepository.Setup(
            r => r.DeleteRange(It.IsAny<IEnumerable<HistoricalContextTimeline>>())
        );
        _mockRepositoryWrapper.Setup(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(1);
        _mockHistoricalContextRepository.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<HistContext, bool>>>(),
                It.IsAny<Func<IQueryable<HistContext>, IIncludableQueryable<HistContext, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(new HistContext
        {
            Id = 1,
            Title = "Context"
        });
        _mockTimelineRepository.Setup(
            r => r.Update(timeline_item_entity)
        );

        // Act
        Result<TimelineItemDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.HistoricalContexts.Should().HaveCount(3);
        _mockHistoricalContextRepository.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<HistContext, bool>>>(),
                It.IsAny<Func<IQueryable<HistContext>, IIncludableQueryable<HistContext, object>>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Exactly(3)
        );
        _mockRepositoryWrapper.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
    }

    [Fact]
    public async Task Handle_ReturnsSuccess_WhenNoHistoricalContextsProvided()
    {
        // Arrange
        TimelineItemDto dto = new()
        {
            Id = 1,
            Title = "Updated Timeline Item",
            Description = "Updated description without contexts.",
            Date = new DateTime(2024, 1, 1),
            DateViewPattern = DateViewPattern.DateMonthYear,
            StreetcodeId = 1,
            HistoricalContexts = []
        };
        TimelineItemEntity timeline_item_entity = new()
        {
            Id = 1,
            Title = "Original Title",
            Description = "Original description.",
            Date = new DateTime(2024, 1, 1),
            DateViewPattern = DateViewPattern.DateMonthYear,
            StreetcodeId = 1,
            HistoricalContextTimelines = []
        };
        UpdateTimelineItemCommand command = new(dto);

        _mockTimelineRepository.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(timeline_item_entity);
        _mockHistoricalContextTimelineRepository.Setup(
            r => r.DeleteRange(It.IsAny<IEnumerable<HistoricalContextTimeline>>())
        );
        _mockRepositoryWrapper.Setup(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(1);
        _mockTimelineRepository.Setup(
            r => r.Update(timeline_item_entity)
        );

        // Act
        Result<TimelineItemDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.HistoricalContexts.Should().BeEmpty();
        _mockTimelineRepository.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        _mockTimelineRepository.Verify(
            r => r.Update(It.IsAny<TimelineItemEntity>()),
            Times.Once
        );
        _mockRepositoryWrapper.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
    }
}
