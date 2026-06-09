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

public sealed class GetTimelineItemByIdHandlerTests
{
    private Mock<IRepositoryWrapper> RepositoryWrapperMock { get; }
    private Mock<ITimelineRepository> TimelineRepositoryMock { get; }
    private Mock<ILoggerService> LoggerMock { get; }
    private IMapper Mapper { get; }
    private GetTimelineItemByIdHandler Handler { get; }

    public GetTimelineItemByIdHandlerTests()
    {
        RepositoryWrapperMock = new Mock<IRepositoryWrapper>();
        TimelineRepositoryMock = new Mock<ITimelineRepository>();
        RepositoryWrapperMock.Setup(r => r.TimelineRepository).Returns(TimelineRepositoryMock.Object);

        LoggerMock = new Mock<ILoggerService>();
        Mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(GetTimelineItemByIdHandler).Assembly);
        }).CreateMapper();
        Handler = new GetTimelineItemByIdHandler(RepositoryWrapperMock.Object, Mapper, LoggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTimelineItem_WhenFound()
    {
        // Arrange
        TimelineItemEntity timeline_item = new()
        {
            Id = 1,
            Title = "Title 2"
        };
        TimelineItemDto dto = Mapper.Map<TimelineItemDto>(timeline_item);
        GetTimelineItemByIdQuery query = new(1);
        TimelineRepositoryMock.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(timeline_item);

        // Act
        Result<TimelineItemDto> result = await Handler.Handle(query, CancellationToken.None);

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
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenItemNotFound()
    {
        // Arrange
        string error_msg = string.Format(ErrorMessages.TypeWithIdNotFound, nameof(TimelineItemEntity), 1);
        TimelineItemEntity timeline_item = new()
        {
            Id = 2,
            Title = "Title 2"
        };
        TimelineItemDto dto = Mapper.Map<TimelineItemDto>(timeline_item);
        GetTimelineItemByIdQuery query = new(1);
        TimelineRepositoryMock.Setup(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>, IIncludableQueryable<TimelineItemEntity, object>>>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync((TimelineItemEntity?)null);
        LoggerMock.Setup(l => l.LogError(query, error_msg));

        // Act
        Result<TimelineItemDto> result = await Handler.Handle(query, CancellationToken.None);

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
        LoggerMock.Verify(l => l.LogError(query, error_msg), Times.Once);
    }
}