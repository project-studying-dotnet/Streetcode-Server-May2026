// <copyright file="GetTimelineItemByStreetcodeIdHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Streetcode.XUnitTest.BLL.MediatR.Timeline.TimelineItem.GetByStreetcodeId
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore.Query;
    using Moq;
    using Streetcode.BLL.DTO.Timeline;
    using Streetcode.BLL.Interfaces.Logging;
    using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetByStreetcodeId;
    using Streetcode.DAL.Entities.Timeline;
    using Streetcode.DAL.Enums;
    using Streetcode.DAL.Repositories.Interfaces.Base;
    using Streetcode.DAL.Repositories.Interfaces.Timeline;
    using Xunit;

    /// <summary>
    /// Unit tests for GetTimelineItemsByStreetcodeIdHandler.
    /// </summary>
    public class GetTimelineItemByStreetcodeIdHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> repoWrapperMock;
        private readonly Mock<ITimelineRepository> timelineRepoMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<ILoggerService> loggerMock;

        private readonly GetTimelineItemsByStreetcodeIdHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetTimelineItemByStreetcodeIdHandlerTests"/> class.
        /// </summary>
        public GetTimelineItemByStreetcodeIdHandlerTests()
        {
            this.repoWrapperMock = new Mock<IRepositoryWrapper>();
            this.timelineRepoMock = new Mock<ITimelineRepository>();
            this.mapperMock = new Mock<IMapper>();
            this.loggerMock = new Mock<ILoggerService>();

            this.repoWrapperMock
                .Setup(x => x.TimelineRepository)
                .Returns(this.timelineRepoMock.Object);

            this.handler = new GetTimelineItemsByStreetcodeIdHandler(
                this.repoWrapperMock.Object,
                this.mapperMock.Object,
                this.loggerMock.Object);
        }

        /// <summary>
        /// Should return TimelineItemDTO when item exists.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnTimelineItemDTO_WhenItemExists()
        {
            var query = new GetTimelineItemsByStreetcodeIdQuery(1);

            var timelineItems = new List<TimelineItem>
            {
                new TimelineItem
                {
                    Id = 1,
                    Title = "Test Title",
                    Description = "Description",
                    Date = new DateTime(2020, 01, 01),
                    DateViewPattern = DateViewPattern.Year,
                    StreetcodeId = 1,
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

            var expectedDtos = new List<TimelineItemDTO>
            {
                new TimelineItemDTO
                {
                    Id = 1,
                    Title = "Test Title",
                    Description = "Description",
                    Date = new DateTime(2020, 01, 01),
                    DateViewPattern = DateViewPattern.Year,
                    HistoricalContexts = new List<HistoricalContextDTO>
                    {
                        new HistoricalContextDTO
                        {
                            Id = 1,
                            Title = "Historical Context 1",
                        },
                    },
                },
            };

            this.timelineRepoMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<TimelineItem, bool>>>(),
                    It.IsAny<Func<IQueryable<TimelineItem>,
                        IIncludableQueryable<TimelineItem, object>>>()))
                .ReturnsAsync(timelineItems);

            this.mapperMock
                .Setup(m => m.Map<IEnumerable<TimelineItemDTO>>(It.IsAny<IEnumerable<TimelineItem>>()))
                .Returns(expectedDtos);

            var result = await this.handler.Handle(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);

            var item = result.Value.First();

            Assert.Equal(1, item.Id);
            Assert.Equal(expectedDtos.First().Title, item.Title);
            Assert.Equal(expectedDtos.First().Description, item.Description);
            Assert.Equal(expectedDtos.First().Date, item.Date);
            Assert.Equal(expectedDtos.First().DateViewPattern, item.DateViewPattern);

            Assert.Single(item.HistoricalContexts);

            Assert.Equal(
                expectedDtos.First().HistoricalContexts.First().Title,
                item.HistoricalContexts.First().Title);

            this.mapperMock.Verify(
                m => m.Map<IEnumerable<TimelineItemDTO>>(
                    It.IsAny<IEnumerable<TimelineItem>>()),
                Times.Once);

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
            var query = new GetTimelineItemsByStreetcodeIdQuery(1);

            this.timelineRepoMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<TimelineItem, bool>>>(),
                    It.IsAny<Func<IQueryable<TimelineItem>,
                        IIncludableQueryable<TimelineItem, object>>>()))
                .ThrowsAsync(new Exception("Database failure"));

            var exception = await Assert.ThrowsAsync<Exception>(() =>
                this.handler.Handle(query, CancellationToken.None));

            Assert.Equal("Database failure", exception.Message);

            this.mapperMock.Verify(
               m => m.Map<IEnumerable<TimelineItemDTO>>(
                   It.IsAny<IEnumerable<TimelineItem>>()),
               Times.Never);

            this.loggerMock.Verify(
                l => l.LogError(
                    It.IsAny<object>(),
                    It.IsAny<string>()),
                Times.Never);

            this.timelineRepoMock.Verify(
                r => r.GetAllAsync(
                    It.IsAny<Expression<Func<TimelineItem, bool>>>(),
                    It.IsAny<Func<IQueryable<TimelineItem>,
                        IIncludableQueryable<TimelineItem, object>>>()),
                Times.Once);
        }

        /// <summary>
        /// Should return empty collection when no timeline items exist.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnEmptyCollection_WhenNoItemsExist()
        {
            var query = new GetTimelineItemsByStreetcodeIdQuery(1);

            this.timelineRepoMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<Expression<Func<TimelineItem, bool>>>(),
                    It.IsAny<Func<IQueryable<TimelineItem>,
                        IIncludableQueryable<TimelineItem, object>>>()))
                .ReturnsAsync(new List<TimelineItem>());

            this.mapperMock
                .Setup(m => m.Map<IEnumerable<TimelineItemDTO>>(
                    It.IsAny<IEnumerable<TimelineItem>>()))
                .Returns(new List<TimelineItemDTO>());

            var result = await this.handler.Handle(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);

            this.mapperMock.Verify(
                m => m.Map<IEnumerable<TimelineItemDTO>>(
                    It.IsAny<IEnumerable<TimelineItem>>()),
                Times.Once);

            this.timelineRepoMock.Verify(
                r => r.GetAllAsync(
                    It.IsAny<Expression<Func<TimelineItem, bool>>>(),
                    It.IsAny<Func<IQueryable<TimelineItem>,
                        IIncludableQueryable<TimelineItem, object>>>()),
                Times.Once);
        }
    }
}
