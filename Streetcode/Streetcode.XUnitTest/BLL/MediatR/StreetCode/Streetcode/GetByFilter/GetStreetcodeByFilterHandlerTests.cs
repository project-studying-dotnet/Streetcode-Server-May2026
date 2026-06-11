using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Filter;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByFilter;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using FactEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Fact;

namespace Streetcode.XUnitTest.BLL.MediatR.Streetcode.Streetcode.GetByFilter;

public class GetStreetcodeByFilterHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GetStreetcodeByFilterHandler _handler;

    public GetStreetcodeByFilterHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _mockLogger = new Mock<ILoggerService>();
        _handler = new GetStreetcodeByFilterHandler(_mockRepo.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WithEmptyRepositories_ReturnsEmptyList()
    {
        var requestDto = new StreetcodeFilterRequestDTO { SearchQuery = "test" };
        var query = new GetStreetcodeByFilterQuery(requestDto);

        _mockRepo.Setup(x => x.StreetcodeRepository.GetAllAsync(
            It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
            It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
            .ReturnsAsync(new List<StreetcodeContent>());

        _mockRepo.Setup(x => x.TextRepository.GetAllAsync(
            It.IsAny<Expression<Func<Text, bool>>>(),
            It.IsAny<Func<IQueryable<Text>, IIncludableQueryable<Text, object>>>()))
            .ReturnsAsync(new List<Text>());

        _mockRepo.Setup(x => x.FactRepository.GetAllAsync(
            It.IsAny<Expression<Func<FactEntity, bool>>>(),
            It.IsAny<Func<IQueryable<FactEntity>, IIncludableQueryable<FactEntity, object>>>()))
            .ReturnsAsync(new List<FactEntity>());

        _mockRepo.Setup(x => x.TimelineRepository.GetAllAsync(
            It.IsAny<Expression<Func<TimelineItem, bool>>>(),
            It.IsAny<Func<IQueryable<TimelineItem>, IIncludableQueryable<TimelineItem, object>>>()))
            .ReturnsAsync(new List<TimelineItem>());

        _mockRepo.Setup(x => x.ArtRepository.GetAllAsync(
            It.IsAny<Expression<Func<Art, bool>>>(),
            It.IsAny<Func<IQueryable<Art>, IIncludableQueryable<Art, object>>>()))
            .ReturnsAsync(new List<Art>());

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenArtMatches_ReturnsCorrectBranches_AndSkipsNullStreetcodes()
    {
        var request = new GetStreetcodeByFilterQuery(new StreetcodeFilterRequestDTO { SearchQuery = "test" });

        _mockRepo.Setup(x => x.StreetcodeRepository.GetAllAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>()))
            .ReturnsAsync(new List<StreetcodeContent>());
        _mockRepo.Setup(x => x.TextRepository.GetAllAsync(It.IsAny<Expression<Func<Text, bool>>>(), It.IsAny<Func<IQueryable<Text>, IIncludableQueryable<Text, object>>>()))
            .ReturnsAsync(new List<Text>());
        _mockRepo.Setup(x => x.FactRepository.GetAllAsync(It.IsAny<Expression<Func<FactEntity, bool>>>(), It.IsAny<Func<IQueryable<FactEntity>, IIncludableQueryable<FactEntity, object>>>()))
            .ReturnsAsync(new List<FactEntity>());
        _mockRepo.Setup(x => x.TimelineRepository.GetAllAsync(It.IsAny<Expression<Func<TimelineItem, bool>>>(), It.IsAny<Func<IQueryable<TimelineItem>, IIncludableQueryable<TimelineItem, object>>>()))
            .ReturnsAsync(new List<TimelineItem>());

        _mockRepo.Setup(r => r.ArtRepository.GetAllAsync(
            It.IsAny<Expression<Func<Art, bool>>>(),
            It.IsAny<Func<IQueryable<Art>, IIncludableQueryable<Art, object>>>()))
            .ReturnsAsync(new List<Art> { new() { Id = 1, Description = "test" } });

        var mockStreetcode = new StreetcodeContent { Id = 1, Title = "Test Art" };

        _mockRepo.Setup(r => r.StreetcodeRepository.GetFirstOrDefaultAsync(
            It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
            null))
            .ReturnsAsync(mockStreetcode);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        result.Value.First().Content.Should().Be("Test Art");
    }
}