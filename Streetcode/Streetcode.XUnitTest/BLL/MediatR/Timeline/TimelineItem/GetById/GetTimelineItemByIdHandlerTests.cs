// <copyright file="GetTimelineItemByIdHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.Timeline;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Mapping.Timeline;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetById;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Timeline;
using Xunit;

using HistContext = Streetcode.DAL.Entities.Timeline.HistoricalContext;
using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.BLL.MediatR.Timeline.TimelineItem.GetById;

public class GetTimelineItemByIdHandlerTests
{
    private const string DatabaseFailureMessage = "Database failure";

    private readonly Mock<IRepositoryWrapper> _repoWrapperMock;
    private readonly Mock<ITimelineRepository> _timelineRepoMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetTimelineItemByIdHandler _handler;

    public GetTimelineItemByIdHandlerTests()
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

        _handler = new GetTimelineItemByIdHandler(
            _repoWrapperMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTimelineItemDTO_WhenItemExists()
    {
        var query = new GetTimelineItemByIdQuery(1);

        var timelineItem = new TimelineItemEntity
        {
            Id = 1,
            Title = "Test Title",
            Description = "Description",
            Date = new DateTime(2020, 1, 1),
            DateViewPattern = DateViewPattern.Year,
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
        };

        _timelineRepoMock
            .Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>,
                    IIncludableQueryable<TimelineItemEntity, object>>>()))
            .ReturnsAsync(timelineItem);

        var result = await _handler.Handle(query, CancellationToken.None);

        var expectedDto = _mapper.Map<TimelineItemDto>(timelineItem);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(expectedDto);

        _timelineRepoMock.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>,
                    IIncludableQueryable<TimelineItemEntity, object>>>()),
            Times.Once);

        _loggerMock.Verify(
            l => l.LogError(
                It.IsAny<object>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnFail_WhenItemNotFound()
    {
        var query = new GetTimelineItemByIdQuery(1);

        var expectedErrorMessage =
            $"Cannot find a timeline item with corresponding id: {query.Id}";

        _timelineRepoMock
            .Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>,
                    IIncludableQueryable<TimelineItemEntity, object>>>()))
            .ReturnsAsync((TimelineItemEntity?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Message.Should().Be(expectedErrorMessage);

        _loggerMock.Verify(
            l => l.LogError(
                It.Is<GetTimelineItemByIdQuery>(q => q.Id == query.Id),
                expectedErrorMessage),
            Times.Once);

        _timelineRepoMock.Verify(
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>,
                    IIncludableQueryable<TimelineItemEntity, object>>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPropagateException_WhenRepositoryThrows()
    {
        var query = new GetTimelineItemByIdQuery(1);

        _timelineRepoMock
            .Setup(r => r.GetFirstOrDefaultAsync(
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
            r => r.GetFirstOrDefaultAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>,
                    IIncludableQueryable<TimelineItemEntity, object>>>()),
            Times.Once);
    }
}
