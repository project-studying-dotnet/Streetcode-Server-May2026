// <copyright file="GetAllTimelineItemHandlerTests.cs" company="PlaceholderCompany">
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
using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetAll;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Interfaces.Timeline;
using Xunit;

using TimelineItemEntity = Streetcode.DAL.Entities.Timeline.TimelineItem;

namespace Streetcode.XUnitTest.BLL.MediatR.Timeline.TimelineItem.GetAll;

public class GetAllTimelineItemHandlerTests
{
    private const string DatabaseFailureMessage = "Database failure";

    private readonly Mock<IRepositoryWrapper> _repoWrapperMock;
    private readonly Mock<ITimelineRepository> _timelineRepoMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILoggerService> _loggerMock;
    private readonly GetAllTimelineItemsHandler _handler;

    public GetAllTimelineItemHandlerTests()
    {
        _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TimelineItemProfile>();
        }).CreateMapper();

        _repoWrapperMock = new Mock<IRepositoryWrapper>();
        _timelineRepoMock = new Mock<ITimelineRepository>();
        _loggerMock = new Mock<ILoggerService>();

        _repoWrapperMock
            .Setup(x => x.TimelineRepository)
            .Returns(_timelineRepoMock.Object);

        _handler = new GetAllTimelineItemsHandler(
            _repoWrapperMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTimelineItemDtos_WhenItemsExist()
    {
        var query = new GetAllTimelineItemsQuery();

        var timelineItems = new List<TimelineItemEntity>
        {
            new()
            {
                Id = 1,
                Title = "Test Title",
                Description = "Description",
                Date = new DateTime(2020, 1, 1),
                DateViewPattern = DateViewPattern.Year,
                HistoricalContextTimelines = new List<HistoricalContextTimeline>
                {
                    new()
                    {
                        HistoricalContext = new HistoricalContext
                        {
                            Id = 1,
                            Title = "Historical Context 1",
                        },
                    },
                },
            },
        };

        _timelineRepoMock
            .Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>,
                    IIncludableQueryable<TimelineItemEntity, object>>>()))
            .ReturnsAsync(timelineItems);

        var result = await _handler.Handle(query, CancellationToken.None);

        var expectedDtos = _mapper.Map<List<TimelineItemDTO>>(timelineItems);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEquivalentTo(expectedDtos);

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
        var query = new GetAllTimelineItemsQuery();

        _timelineRepoMock
            .Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>,
                    IIncludableQueryable<TimelineItemEntity, object>>>()))
            .ThrowsAsync(new Exception(DatabaseFailureMessage));

        Func<Task> act = () => _handler.Handle(query, CancellationToken.None);

        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage(DatabaseFailureMessage);

        _loggerMock.Verify(
            l => l.LogError(
                It.IsAny<object>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyCollection_WhenNoItemsExist()
    {
        var query = new GetAllTimelineItemsQuery();

        _timelineRepoMock
            .Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>,
                    IIncludableQueryable<TimelineItemEntity, object>>>()))
            .ReturnsAsync(new List<TimelineItemEntity>());

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();

        _timelineRepoMock.Verify(
            r => r.GetAllAsync(
                It.IsAny<Expression<Func<TimelineItemEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItemEntity>,
                    IIncludableQueryable<TimelineItemEntity, object>>>()),
            Times.Once);
    }
}