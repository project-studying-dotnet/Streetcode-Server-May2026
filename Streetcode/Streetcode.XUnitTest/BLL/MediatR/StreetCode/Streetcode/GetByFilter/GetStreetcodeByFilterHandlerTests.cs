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
using Streetcode.DAL.Specifications.Base;
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

    private void SetupEmptyRepositories()
    {
        _mockRepo.Setup(x => x.StreetcodeRepository.GetAllAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>())).ReturnsAsync(new List<StreetcodeContent>());
        _mockRepo.Setup(x => x.StreetcodeRepository.GetAllAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null)).ReturnsAsync(new List<StreetcodeContent>());
        _mockRepo.Setup(x => x.TextRepository.GetAllAsync(It.IsAny<Expression<Func<Text, bool>>>(), It.IsAny<Func<IQueryable<Text>, IIncludableQueryable<Text, object>>>())).ReturnsAsync(new List<Text>());
        _mockRepo.Setup(x => x.TextRepository.GetAllAsync(It.IsAny<Expression<Func<Text, bool>>>(), null)).ReturnsAsync(new List<Text>());
        _mockRepo.Setup(x => x.FactRepository.GetAllAsync(It.IsAny<Expression<Func<FactEntity, bool>>>(), It.IsAny<Func<IQueryable<FactEntity>, IIncludableQueryable<FactEntity, object>>>())).ReturnsAsync(new List<FactEntity>());
        _mockRepo.Setup(x => x.FactRepository.GetAllAsync(It.IsAny<Expression<Func<FactEntity, bool>>>(), null)).ReturnsAsync(new List<FactEntity>());
        _mockRepo.Setup(x => x.TimelineRepository.GetAllAsync(It.IsAny<Expression<Func<TimelineItem, bool>>>(), It.IsAny<Func<IQueryable<TimelineItem>, IIncludableQueryable<TimelineItem, object>>>())).ReturnsAsync(new List<TimelineItem>());
        _mockRepo.Setup(x => x.TimelineRepository.GetAllAsync(It.IsAny<Expression<Func<TimelineItem, bool>>>(), null)).ReturnsAsync(new List<TimelineItem>());
        _mockRepo.Setup(x => x.ArtRepository.GetAllAsync(It.IsAny<ISpecification<Art>>())).ReturnsAsync(new List<Art>());
    }

    [Fact]
    public async Task Handle_WithEmptyRepositories_ReturnsEmptyList()
    {
        SetupEmptyRepositories();
        var query = new GetStreetcodeByFilterQuery(new StreetcodeFilterRequestDTO { SearchQuery = "test" });

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_StreetcodeMatchesAllFields_ReturnsResults()
    {
        SetupEmptyRepositories();
        var query = new GetStreetcodeByFilterQuery(new StreetcodeFilterRequestDTO { SearchQuery = "test" });

        var streetcodes = new List<StreetcodeContent>
        {
            new() { Id = 1, Title = "test-title", Alias = "no", Teaser = "no", TransliterationUrl = "no" },
            new() { Id = 2, Title = "no", Alias = "test-alias", Teaser = "no", TransliterationUrl = "no" },
            new() { Id = 3, Title = "no", Alias = "no", Teaser = "test-teaser", TransliterationUrl = "no" },
            new() { Id = 4, Title = "no", Alias = "no", Teaser = "no", TransliterationUrl = "test-url" }
        };

        _mockRepo.Setup(x => x.StreetcodeRepository.GetAllAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>())).ReturnsAsync(streetcodes);
        _mockRepo.Setup(x => x.StreetcodeRepository.GetAllAsync(It.IsAny<Expression<Func<StreetcodeContent, bool>>>(), null)).ReturnsAsync(streetcodes);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(4);
    }

    [Fact]
    public async Task Handle_TextMatchesAllFields_ReturnsResults()
    {
        SetupEmptyRepositories();
        var query = new GetStreetcodeByFilterQuery(new StreetcodeFilterRequestDTO { SearchQuery = "test" });

        var texts = new List<Text>
        {
            new() { Id = 1, Title = "test-title", TextContent = "no", Streetcode = new StreetcodeContent { Id = 1 } },
            new() { Id = 2, Title = "no", TextContent = "test-content", Streetcode = new StreetcodeContent { Id = 2 } }
        };

        _mockRepo.Setup(x => x.TextRepository.GetAllAsync(It.IsAny<Expression<Func<Text, bool>>>(), It.IsAny<Func<IQueryable<Text>, IIncludableQueryable<Text, object>>>())).ReturnsAsync(texts);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_FactMatchesAllFields_ReturnsResults()
    {
        SetupEmptyRepositories();
        var query = new GetStreetcodeByFilterQuery(new StreetcodeFilterRequestDTO { SearchQuery = "test" });

        var facts = new List<FactEntity>
        {
            new() { Id = 1, Title = "test-title", FactContent = "no", Streetcode = new StreetcodeContent { Id = 1 } },
            new() { Id = 2, Title = "no", FactContent = "test-content", Streetcode = new StreetcodeContent { Id = 2 } }
        };

        _mockRepo.Setup(x => x.FactRepository.GetAllAsync(It.IsAny<Expression<Func<FactEntity, bool>>>(), It.IsAny<Func<IQueryable<FactEntity>, IIncludableQueryable<FactEntity, object>>>())).ReturnsAsync(facts);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_TimelineMatchesAllFields_ReturnsResults()
    {
        SetupEmptyRepositories();
        var query = new GetStreetcodeByFilterQuery(new StreetcodeFilterRequestDTO { SearchQuery = "test" });

        var timelines = new List<TimelineItem>
        {
            new() { Id = 1, Title = "test-title", Description = "no", Streetcode = new StreetcodeContent { Id = 1 } },
            new() { Id = 2, Title = "no", Description = "test-desc", Streetcode = new StreetcodeContent { Id = 2 } }
        };

        _mockRepo.Setup(x => x.TimelineRepository.GetAllAsync(It.IsAny<Expression<Func<TimelineItem, bool>>>(), It.IsAny<Func<IQueryable<TimelineItem>, IIncludableQueryable<TimelineItem, object>>>())).ReturnsAsync(timelines);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ArtMatches_ReturnsResults()
    {
        SetupEmptyRepositories();
        var query = new GetStreetcodeByFilterQuery(new StreetcodeFilterRequestDTO { SearchQuery = "test" });

        var arts = new List<Art>
        {
            new()
            {
                Id = 1,
                Description = "test-desc",
                StreetcodeArts = new List<StreetcodeArt>
                {
                    new() { Streetcode = new StreetcodeContent { Id = 1 } }
                }
            }
        };

        _mockRepo.Setup(x => x.ArtRepository.GetAllAsync(It.IsAny<ISpecification<Art>>())).ReturnsAsync(arts);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ArtMatches_WithNullStreetcode_SkipsResult()
    {
        SetupEmptyRepositories();
        var query = new GetStreetcodeByFilterQuery(new StreetcodeFilterRequestDTO { SearchQuery = "test" });

        var arts = new List<Art>
        {
            new()
            {
                Id = 1,
                Description = "test-desc",
                StreetcodeArts = new List<StreetcodeArt>
                {
                    new() { Streetcode = null! }
                }
            }
        };

        _mockRepo.Setup(x => x.ArtRepository.GetAllAsync(It.IsAny<ISpecification<Art>>())).ReturnsAsync(arts);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}