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

    /// <summary>
    /// Unit tests for GetTimelineItemByIdHandler.
    /// </summary>
    public class GetTimelineItemByIdHandlerTests
    {
        private readonly Mock<IRepositoryWrapper> repoWrapperMock;
        private readonly Mock<ITimelineRepository> timelineRepoMock;
        private readonly IMapper mapper;
        private readonly Mock<ILoggerService> loggerMock;

        private readonly GetTimelineItemByIdHandler handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetTimelineItemByIdHandlerTests"/> class.
        /// </summary>
        public GetTimelineItemByIdHandlerTests()
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

            this.handler = new GetTimelineItemByIdHandler(
                this.repoWrapperMock.Object,
                this.mapper,
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

            var timelineItem = new TimelineItem
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

            this.timelineRepoMock
                .Setup(r => r.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<TimelineItem, bool>>>(),
                    It.IsAny<Func<IQueryable<TimelineItem>,
                        IIncludableQueryable<TimelineItem, object>>>()))
                .ReturnsAsync(timelineItem);

            var result = await this.handler.Handle(query, CancellationToken.None);

            var expectedDto = this.mapper.Map<TimelineItemDto>(timelineItem);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().BeEquivalentTo(expectedDto);

            this.timelineRepoMock.Verify(
                r => r.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<TimelineItem, bool>>>(),
                    It.IsAny<Func<IQueryable<TimelineItem>,
                        IIncludableQueryable<TimelineItem, object>>>()),
                Times.Once);

            this.loggerMock.Verify(
                l => l.LogError(It.IsAny<object>(), It.IsAny<string>()),
                Times.Never);
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
                    It.IsAny<Expression<Func<TimelineItem, bool>>>(),
                    It.IsAny<Func<IQueryable<TimelineItem>,
                        IIncludableQueryable<TimelineItem, object>>>()))
                .ReturnsAsync((TimelineItem?)null);

            var result = await this.handler.Handle(query, CancellationToken.None);

            result.IsFailed.Should().BeTrue();
            result.Errors.First().Message.Should().Be(expectedErrorMessage);

            this.loggerMock.Verify(
                l => l.LogError(
                    It.Is<GetTimelineItemByIdQuery>(q => q.Id == query.Id),
                    expectedErrorMessage),
                Times.Once);

            this.timelineRepoMock.Verify(
                r => r.GetFirstOrDefaultAsync(
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
            var query = new GetTimelineItemByIdQuery(1);

            this.timelineRepoMock
                .Setup(r => r.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<TimelineItem, bool>>>(),
                    It.IsAny<Func<IQueryable<TimelineItem>,
                        IIncludableQueryable<TimelineItem, object>>>()))
                .ThrowsAsync(new Exception("Database failure"));

            var exception = await Assert.ThrowsAsync<Exception>(() =>
                this.handler.Handle(query, CancellationToken.None));

            exception.Message.Should().Be("Database failure");

            this.loggerMock.Verify(
                l => l.LogError(
                    It.IsAny<object>(),
                    It.IsAny<string>()),
                Times.Never);

            this.timelineRepoMock.Verify(
                r => r.GetFirstOrDefaultAsync(
                    It.IsAny<Expression<Func<TimelineItem, bool>>>(),
                    It.IsAny<Func<IQueryable<TimelineItem>,
                        IIncludableQueryable<TimelineItem, object>>>()),
                Times.Once);
        }
    }
}
