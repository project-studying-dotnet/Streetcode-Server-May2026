using System.Linq.Expressions;
using Moq;
using Xunit;
using AutoMapper;
using FluentResults;
using FluentAssertions;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Microsoft.EntityFrameworkCore.Query;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.MediatR.Timeline.TimelineItem;
using Streetcode.DAL.Repositories.Interfaces.Timeline;
using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.BLL.MediatR.Timeline.TimelineItem;

public sealed class DeleteTimelineItemHandlerTests
{
    private Mock<IRepositoryWrapper> RepositoryWrapperMock { get; }
    private Mock<ITimelineRepository> TimelineRepositoryMock { get; }
    private Mock<ILoggerService> LoggerMock { get; }
    private IMapper Mapper { get; }
    private DeleteTimelineItemHandler Handler { get; }

    public DeleteTimelineItemHandlerTests()
    {
        RepositoryWrapperMock = new Mock<IRepositoryWrapper>();
        TimelineRepositoryMock = new Mock<ITimelineRepository>();
        RepositoryWrapperMock.Setup(r => r.TimelineRepository).Returns(TimelineRepositoryMock.Object);

        LoggerMock = new Mock<ILoggerService>();
        Mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(DeleteTimelineItemHandler).Assembly);
        }).CreateMapper();
        Handler = new DeleteTimelineItemHandler(RepositoryWrapperMock.Object, Mapper, LoggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsTimelineItem_WhenDeletedSuccessfully()
    {
        // Arrange
        TimelineItemEntity timeline_item = new()
        {
            Id = 0,
            Title = "Test Timeline Item",
        };
        TimelineItemDto dto = Mapper.Map<TimelineItemDto>(timeline_item);
        DeleteTimelineItemCommand command = new(0);
        TimelineRepositoryMock.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(timeline_item);
        TimelineRepositoryMock.Setup(
            r => r.Delete(It.IsAny<TimelineItemEntity>())
        );
        RepositoryWrapperMock.Setup(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>())
        ).ReturnsAsync(1);

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
            r => r.Delete(It.IsAny<TimelineItemEntity>()),
            Times.Once
        );
        RepositoryWrapperMock.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenDeleteFails()
    {
        // Arrange
        string error_msg = string.Format(ErrorMessages.FailedToDeleteType, nameof(TimelineItemEntity));
        TimelineItemEntity timeline_item = new()
        {
            Id = 0,
            Title = "Test Timeline Item",
        };
        TimelineItemDto dto = Mapper.Map<TimelineItemDto>(timeline_item);
        DeleteTimelineItemCommand command = new(0);
        TimelineRepositoryMock.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(timeline_item);
        TimelineRepositoryMock.Setup(
            r => r.Delete(It.IsAny<TimelineItemEntity>())
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
            r => r.Delete(It.IsAny<TimelineItemEntity>()),
            Times.Once
        );
        RepositoryWrapperMock.Verify(
            r => r.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        LoggerMock.Verify(l => l.LogError(command, error_msg), Times.Once);
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenItemNotFound()
    {
        // Arrange
        TimelineItemEntity timeline_item = new()
        {
            Id = 1,
            Title = "Test Timeline Item",
        };
        TimelineItemDto dto = Mapper.Map<TimelineItemDto>(timeline_item);
        DeleteTimelineItemCommand command = new(0);
        string error_msg = string.Format(ErrorMessages.TypeWithIdNotFound, nameof(TimelineItemEntity), command.Id);
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
}