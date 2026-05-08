// <copyright file="GetTimelineItemByIdHandlerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace Streetcode.XUnitTest.BLL.MediatR.Timeline.TimelineItem.GetById
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
    using Streetcode.BLL.MediatR.Timeline.TimelineItem.GetById;
    using Streetcode.DAL.Entities.Timeline;
    using Streetcode.DAL.Enums;
    using Streetcode.DAL.Repositories.Interfaces.Base;
    using Streetcode.DAL.Repositories.Interfaces.Timeline;
    using Xunit;

    /// <summary>
    /// Unit tests for GetTimelineItemByIdHandler.
    /// </summary>
    public class GetTimelineItemByIdHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> repoWrapperMock;
        private readonly Mock<ITimelineRepository> timelineRepoMock;
        private readonly Mock<IMapper> mapperMock;
        private readonly Mock<ILoggerService> loggerMock;

        private readonly GetTimelineItemByIdHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetTimelineItemByIdHandlerTests"/> class.
        /// </summary>
        public GetTimelineItemByIdHandlerTests()
        {
            this.repoWrapperMock = new Mock<IRepositoryWrapper>();
            this.timelineRepoMock = new Mock<ITimelineRepository>();
            this.mapperMock = new Mock<IMapper>();
            this.loggerMock = new Mock<ILoggerService>();

            this.repoWrapperMock
                .Setup(x => x.TimelineRepository)
                .Returns(this.timelineRepoMock.Object);

            this.handler = new GetTimelineItemByIdHandler(
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
            var query = new GetTimelineItemByIdQuery(1);

            var timelineItem = new DAL.Entities.Timeline.TimelineItem
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
            };

            var expectedDto = new TimelineItemDTO
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
            };

            this.timelineRepoMock
                .Setup(r => r.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<DAL.Entities.Timeline.TimelineItem, bool>>>(),
                    It.IsAny<Func<IQueryable<DAL.Entities.Timeline.TimelineItem>,
                        IIncludableQueryable<DAL.Entities.Timeline.TimelineItem, object>>>()))
                .ReturnsAsync(timelineItem);

            this.mapperMock
                .Setup(m => m.Map<TimelineItemDTO>(timelineItem))
                .Returns(expectedDto);

            var result = await this.handler.Handle(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);

            Assert.Equal(expectedDto.Id, result.Value.Id);
            Assert.Equal(expectedDto.Title, result.Value.Title);
            Assert.Equal(expectedDto.Description, result.Value.Description);
            Assert.Equal(expectedDto.Date, result.Value.Date);
            Assert.Equal(expectedDto.DateViewPattern, result.Value.DateViewPattern);

            Assert.NotNull(result.Value.HistoricalContexts);
            Assert.Single(result.Value.HistoricalContexts);

            Assert.Equal(
                expectedDto.HistoricalContexts.First().Title,
                result.Value.HistoricalContexts.First().Title);

            this.mapperMock.Verify(
                m => m.Map<TimelineItemDTO>(timelineItem),
                Times.Once);
        }

        /// <summary>
        /// Should return failure when item not found.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldReturnFail_WhenItemNotFound()
        {
            var query = new GetTimelineItemByIdQuery(1);

            var expectedErrorMessage =
                $"Cannot find a timeline item with corresponding id: {query.Id}";

            this.timelineRepoMock
                .Setup(r => r.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<DAL.Entities.Timeline.TimelineItem, bool>>>(),
                    It.IsAny<Func<IQueryable<DAL.Entities.Timeline.TimelineItem>,
                        IIncludableQueryable<DAL.Entities.Timeline.TimelineItem, object>>>()))
                .ReturnsAsync((DAL.Entities.Timeline.TimelineItem?)null);

            var result = await this.handler.Handle(query, CancellationToken.None);

            Assert.True(result.IsFailed);

            Assert.Equal(
                expectedErrorMessage,
                result.Errors.First().Message);

            this.loggerMock.Verify(
                l => l.LogError(
                    It.Is<GetTimelineItemByIdQuery>(q => q.Id == query.Id),
                    expectedErrorMessage),
                Times.Once);

            this.mapperMock.Verify(
                m => m.Map<TimelineItemDTO>(
                    It.IsAny<DAL.Entities.Timeline.TimelineItem>()),
                Times.Never);
        }

        /// <summary>
        /// Should throw exception when repository fails.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        [Fact]
        public async Task Handle_ShouldThrowException_WhenRepositoryFails()
        {
            var query = new GetTimelineItemByIdQuery(1);

            this.timelineRepoMock
                .Setup(r => r.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<DAL.Entities.Timeline.TimelineItem, bool>>>(),
                    It.IsAny<Func<IQueryable<DAL.Entities.Timeline.TimelineItem>,
                        IIncludableQueryable<DAL.Entities.Timeline.TimelineItem, object>>>()))
                .ThrowsAsync(new Exception("Database failure"));

            var exception = await Assert.ThrowsAsync<Exception>(() =>
                this.handler.Handle(query, CancellationToken.None));

            Assert.Equal("Database failure", exception.Message);

            this.mapperMock.Verify(
                m => m.Map<TimelineItemDTO>(
                    It.IsAny<DAL.Entities.Timeline.TimelineItem>()),
                Times.Never);

            this.loggerMock.Verify(
                l => l.LogError(
                    It.IsAny<object>(),
                    It.IsAny<string>()),
                Times.Never);
        }
    }
}
