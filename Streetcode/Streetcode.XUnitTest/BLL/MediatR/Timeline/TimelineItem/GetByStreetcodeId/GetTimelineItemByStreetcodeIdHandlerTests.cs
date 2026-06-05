// <copyright file="GetTimelineItemByStreetcodeIdHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Timeline;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetByStreetcodeId;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Timeline;
using Xunit;

using HistContext = Streetcode.DAL.Entities.Timeline.HistoricalContext;
using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.BLL.MediatR.Timeline.TimelineItem.GetByStreetcodeId;

public class GetTimelineItemByStreetcodeIdHandlerTests
{
    private const string DatabaseFailureMessage = "Database failure";

    private readonly Mock<IRepositoryWrapper> _repoWrapperMock;
    private readonly Mock<ITimelineRepository> _timelineRepoMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetTimelineItemsByStreetcodeIdHandler _handler;

    public GetTimelineItemByStreetcodeIdHandlerTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TimelineItemProfile>();
        });

        _mapper = config.CreateMapper();

        _repoWrapperMock = new Mock<IRepositoryWrapper>();
        _timelineRepoMock = new Mock<ITimelineRepository>();
        _loggerMock = new Mock<ILoggerService>();

        _repoWrapperMock
            .Setup(x => x.TimelineRepository)
            .Returns(_timelineRepoMock.Object);

        _handler = new GetTimelineItemsByStreetcodeIdHandler(
            _repoWrapperMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTimelineItemDTO_WhenItemExists()
    {
        var query = new GetTimelineItemsByStreetcodeIdQuery(1);

        var timelineItems = new List<TimelineItemEntity>
        {
            new()
            {
                Id = 1,
                Title = "Test Title",
                Description = "Description",
                Date = new DateTime(2020, 1, 1),
                DateViewPattern = DateViewPattern.Year,
                StreetcodeId = 1,
                HistoricalContextTimelines = [
                    new HistoricalContextTimeline()
                    {
                        HistoricalContext = new HistContext
                        {
                            Id = 1,
                            Title = "Historical Context 1",
                        }
                    }
                ]
            },
        };

        _timelineRepoMock
            .Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>,
                    IIncludableQueryable<TimelineItemEntity, object>>>()))
            .ReturnsAsync(timelineItems);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);

        var item = result.Value.First();

        item.Id.Should().Be(1);
        item.Title.Should().Be("Test Title");
        item.Description.Should().Be("Description");
        item.DateViewPattern.Should().Be(DateViewPattern.Year);

        item.HistoricalContexts.Should().HaveCount(1);
        item.HistoricalContexts.First().Title.Should().Be("Historical Context 1");

        _timelineRepoMock.Verify(
            r => r.GetAllAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>,
                    IIncludableQueryable<TimelineItemEntity, object>>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPropagateException_WhenRepositoryThrows()
    {
        var query = new GetTimelineItemsByStreetcodeIdQuery(1);

        _timelineRepoMock
            .Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>,
                    IIncludableQueryable<TimelineItemEntity, object>>>()))
            .ThrowsAsync(new Exception(DatabaseFailureMessage));

        Func<Task> act = () =>
            _handler.Handle(query, CancellationToken.None);

        var exception = await act.Should()
            .ThrowAsync<Exception>();

        exception.Which.Message.Should().Be(DatabaseFailureMessage);

        _loggerMock.Verify(
            l => l.LogError(
                It.IsAny<object>(),
                It.IsAny<string>()),
            Times.Never);

        _timelineRepoMock.Verify(
            r => r.GetAllAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>,
                    IIncludableQueryable<TimelineItemEntity, object>>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyCollection_WhenNoItemsExist()
    {
        var query = new GetTimelineItemsByStreetcodeIdQuery(1);

        _timelineRepoMock
            .Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>,
                    IIncludableQueryable<TimelineItemEntity, object>>>()))
            .ReturnsAsync(new List<TimelineItemEntity>());

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();

        _timelineRepoMock.Verify(
            r => r.GetAllAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>,
                    IIncludableQueryable<TimelineItemEntity, object>>>()),
            Times.Once);
    }
}
