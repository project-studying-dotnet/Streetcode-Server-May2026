using System.Linq.Expressions;
using Moq;
using Xunit;
using AutoMapper;
using FluentResults;
using FluentAssertions;
using MockQueryable.Moq;
using Streetcode.BLL.Resources;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.MediatR.Timeline.TimelineItem;
using Streetcode.DAL.Repositories.Interfaces.Timeline;
using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.BLL.MediatR.Timeline.TimelineItem;

public sealed class GetTimelineItemsByStreetcodeIdHandlerTests
{
    private Mock<IRepositoryWrapper> RepositoryWrapperMock { get; }
    private Mock<ITimelineRepository> TimelineRepositoryMock { get; }
    private Mock<ILoggerService> LoggerMock { get; }
    private IMapper Mapper { get; }
    private GetTimelineItemsByStreetcodeIdHandler Handler { get; }

    public GetTimelineItemsByStreetcodeIdHandlerTests()
    {
        RepositoryWrapperMock = new Mock<IRepositoryWrapper>();
        TimelineRepositoryMock = new Mock<ITimelineRepository>();
        RepositoryWrapperMock.Setup(r => r.TimelineRepository).Returns(TimelineRepositoryMock.Object);

        LoggerMock = new Mock<ILoggerService>();
        Mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddMaps(typeof(GetTimelineItemsByStreetcodeIdHandler).Assembly);
        }).CreateMapper();
        Handler = new GetTimelineItemsByStreetcodeIdHandler(RepositoryWrapperMock.Object, Mapper, LoggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTimelineItems_WhenItemsFound()
    {
        // Arrange
        List<TimelineItemEntity> timeline_items = [
            new TimelineItemEntity()
            {
                Id = 0,
                Title = "Title 1",
                StreetcodeId = 1
            },
            new TimelineItemEntity()
            {
                Id = 1,
                Title = "Title 2",
                StreetcodeId = 1
            }
        ];
        List<TimelineItemDto> dtos = Mapper.Map<IEnumerable<TimelineItemDto>>(timeline_items).ToList();
        GetTimelineItemsByStreetcodeIdQuery query = new(1);
        TimelineRepositoryMock.Setup(
            r => r.FindAll(It.IsAny<Expression<Func<TimelineItemEntity, bool>>>())
        ).Returns(timeline_items.BuildMock());

        // Act
        Result<IEnumerable<TimelineItemDto>> result = await Handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(dtos);
        TimelineRepositoryMock.Verify(
            r => r.FindAll(It.IsAny<Expression<Func<TimelineItemEntity, bool>>>()),
            Times.Once
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenNoItemsExist()
    {
        // Arrange
        string error_msg = string.Format(ErrorMessages.FailedToFindAnyType, nameof(TimelineItemEntity));
        List<TimelineItemEntity> timeline_items = [];
        List<TimelineItemDto> dtos = Mapper.Map<IEnumerable<TimelineItemDto>>(timeline_items).ToList();
        GetTimelineItemsByStreetcodeIdQuery query = new(1);
        TimelineRepositoryMock.Setup(
            r => r.FindAll(It.IsAny<Expression<Func<TimelineItemEntity, bool>>>())
        ).Returns(timeline_items.BuildMock());
        LoggerMock.Setup(l => l.LogError(query, error_msg));

        // Act
        Result<IEnumerable<TimelineItemDto>> result = await Handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().BeEquivalentTo([
            new Error(error_msg)
        ]);
        TimelineRepositoryMock.Verify(
            r => r.FindAll(It.IsAny<Expression<Func<TimelineItemEntity, bool>>>()),
            Times.Once
        );
        LoggerMock.Verify(l => l.LogError(query, error_msg), Times.Once);
    }
}