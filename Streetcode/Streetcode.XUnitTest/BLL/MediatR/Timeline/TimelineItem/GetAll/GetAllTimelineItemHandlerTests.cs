// <copyright file="GetAllTimelineItemHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Streetcode.XUnitTest.BLL.MediatR.Timeline.TimelineItem.GetAll
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
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

    /// <summary>
    /// Unit tests for GetAllTimelineItemsHandler.
    /// </summary>
    public class GetAllTimelineItemHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> repoWrapperMock;
        private readonly Mock<ITimelineRepository> timelineRepoMock;
        private readonly IMapper mapper;
        private readonly Mock<ILoggerService> loggerMock;

        private readonly GetAllTimelineItemsHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAllTimelineItemHandlerTests"/> class.
        /// </summary>
        public GetAllTimelineItemHandlerTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<TimelineItemProfile>();
            });

            this.mapper = config.CreateMapper();

            this.repoWrapperMock = new Mock<IRepositoryWrapper>();
            this.timelineRepoMock = new Mock<ITimelineRepository>();
            this.loggerMock = new Mock<ILoggerService>();

            this.repoWrapperMock
                .Setup(x => x.TimelineRepository)
                .Returns(this.timelineRepoMock.Object);

            this.handler = new GetAllTimelineItemsHandler(
                this.repoWrapperMock.Object,
                this.mapper,
                this.loggerMock.Object);
        }

        /// <summary>
        /// Should return timeline item DTO collection when items exist.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnTimelineItemDtos_WhenItemsExist()
        {
            var query = new GetAllTimelineItemsQuery();

            var timelineItems = new List<TimelineItem>
            {
                new TimelineItem
                {
                    Id = 1,
                    Title = "Test Title",
                    Description = "Description",
                    Date = new DateTime(2020, 01, 01),
                    DateViewPattern = DateViewPattern.Year,
                    HistoricalContextTimelines = new List<HistoricalContextTimeline>
                    {
                        new HistoricalContextTimeline
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

            this.timelineRepoMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<TimelineItem, bool>>>(),
                    It.IsAny<Func<IQueryable<TimelineItem>, IIncludableQueryable<TimelineItem, object>>>()))
            .ReturnsAsync(timelineItems);

            var result = await this.handler.Handle(query, CancellationToken.None);

            var expectedDtos = this.mapper.Map<List<TimelineItemDTO>>(timelineItems);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(expectedDtos);

            this.timelineRepoMock.Verify(
                r => r.GetAllAsync(
                    It.IsAny<Expression<Func<TimelineItem, bool>>>(),
                    It.IsAny<Func<IQueryable<TimelineItem>,
                        IIncludableQueryable<TimelineItem, object>>>()),
                Times.Once);
        }

        /// <summary>
        /// Should throw exception when repository fails.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldPropagateException_WhenRepositoryThrows()
        {
            var query = new GetAllTimelineItemsQuery();

            this.timelineRepoMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<TimelineItem, bool>>>(),
                    It.IsAny<Func<IQueryable<TimelineItem>,
                        IIncludableQueryable<TimelineItem, object>>>()))
                .ThrowsAsync(new Exception("Database failure"));

            Func<Task> act = () => this.handler.Handle(query, CancellationToken.None);

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Database failure");

            this.loggerMock.Verify(
                l => l.LogError(
                    It.IsAny<object>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        /// <summary>
        /// Should return empty collection when no timeline items exist.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnEmptyCollection_WhenNoItemsExist()
        {
            var query = new GetAllTimelineItemsQuery();

            this.timelineRepoMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<TimelineItem, bool>>>(),
                    It.IsAny<Func<IQueryable<TimelineItem>,
                        IIncludableQueryable<TimelineItem, object>>>()))
                .ReturnsAsync(new List<TimelineItem>());

            var result = await this.handler.Handle(query, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().BeEmpty();

            this.timelineRepoMock.Verify(
                r => r.GetAllAsync(
                    It.IsAny<Expression<Func<TimelineItem, bool>>>(),
                    It.IsAny<Func<IQueryable<TimelineItem>,
                        IIncludableQueryable<TimelineItem, object>>>()),
                Times.Once);
        }
    }
}
