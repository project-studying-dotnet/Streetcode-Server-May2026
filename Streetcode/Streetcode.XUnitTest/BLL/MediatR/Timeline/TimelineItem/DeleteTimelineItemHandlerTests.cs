using System.Linq.Expressions;
using Moq;
using Xunit;
using AutoMapper;
using FluentResults;
using FluentAssertions;
using Streetcode.DAL.Enums;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Timeline;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Delete;
using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.BLL.MediatR.Timeline.TimelineItem;

public sealed class DeleteTimelineItemHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ITimelineRepository> _mockTimelineRepository;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly IMapper _mapper;
    private readonly DeleteTimelineItemHandler _handler;

    public DeleteTimelineItemHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockTimelineRepository = new Mock<ITimelineRepository>();
        _mockRepositoryWrapper.Setup(r => r.TimelineRepository).Returns(_mockTimelineRepository.Object);

        _mockLogger = new Mock<ILoggerService>();
        MapperConfiguration config = new(cfg =>
        {
            cfg.AddMaps(typeof(DeleteTimelineItemHandler).Assembly);
        });
        _mapper = config.CreateMapper();
        _handler = new DeleteTimelineItemHandler(_mockRepositoryWrapper.Object, _mapper, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ReturnsDeletedTimelineItem_WhenDeletedSuccessfully()
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
            HistoricalContexts = []
        };
        TimelineItemEntity context = _mapper.Map<TimelineItemEntity>(dto);
        DeleteTimelineItemCommand command = new(dto.Id);
        _mockTimelineRepository.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(context);
        _mockTimelineRepository.Setup(
            r => r.Delete(context)
        );
        _mockRepositoryWrapper.Setup(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(1);

        // Act
        Result<TimelineItemDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        dto.Should().BeEquivalentTo(result.Value);
        _mockTimelineRepository.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        _mockTimelineRepository.Verify(
            r => r.Delete(context),
            Times.Once
        );
        _mockRepositoryWrapper.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenDeleteFails()
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
                    Id = 1,
                    Title = "Context 1",
                }
            ]
        };
        TimelineItemEntity context = _mapper.Map<TimelineItemEntity>(dto);
        DeleteTimelineItemCommand command = new(dto.Id);
        _mockTimelineRepository.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(context);
        _mockTimelineRepository.Setup(
            r => r.Delete(context)
        );
        _mockRepositoryWrapper.Setup(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(0);
        _mockLogger.Setup(
            l => l.LogError(command, string.Format(ErrorMessages.FailedToDeleteTimelineItemWithId, dto.Id))
        );

        // Act
        Result<TimelineItemDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo([
            new Error(string.Format(ErrorMessages.FailedToDeleteTimelineItemWithId, dto.Id))
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
            r => r.Delete(context),
            Times.Once
        );
        _mockRepositoryWrapper.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        _mockLogger.Verify(
            l => l.LogError(command, string.Format(ErrorMessages.FailedToDeleteTimelineItemWithId, dto.Id)),
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
        TimelineItemEntity context = _mapper.Map<TimelineItemEntity>(dto);
        DeleteTimelineItemCommand command = new(dto.Id);
        _mockTimelineRepository.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(context);
        _mockTimelineRepository.Setup(
            r => r.Delete(context)
        );
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
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
        _mockTimelineRepository.Verify(
            r => r.Delete(context),
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
                    Id = 1,
                    Title = "Context 1",
                }
            ]
        };
        TimelineItemEntity context = _mapper.Map<TimelineItemEntity>(dto);
        DeleteTimelineItemCommand command = new(dto.Id);
        _mockTimelineRepository.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync((TimelineItemEntity?)null);
        _mockLogger.Setup(
            l => l.LogError(command, string.Format(ErrorMessages.TimelineItemWithIdNotFound, command.Id))
        );

        // Act
        Result<TimelineItemDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo([
            new Error(string.Format(ErrorMessages.TimelineItemWithIdNotFound, command.Id))
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
            l => l.LogError(command, string.Format(ErrorMessages.TimelineItemWithIdNotFound, command.Id)),
            Times.Once
        );
    }
}