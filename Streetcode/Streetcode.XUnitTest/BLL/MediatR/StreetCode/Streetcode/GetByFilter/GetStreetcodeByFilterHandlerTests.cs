using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Streetcode.BLL.DTO.AdditionalContent.Filter;
using Streetcode.BLL.DTO.Streetcode;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Streetcode.GetByFilter;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Xunit;
using FactEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Fact;
using TextEntity = Streetcode.DAL.Entities.Streetcode.TextContent.Text;

namespace Streetcode.XUnitTest.BLL.MediatR.StreetCode.Streetcode.GetByFilter;

public class GetStreetcodeByFilterHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepositoryWrapper;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GetStreetcodeByFilterHandler _handler;

    public GetStreetcodeByFilterHandlerTests()
    {
        _mockRepositoryWrapper = new Mock<IRepositoryWrapper>();
        _mockLogger = new Mock<ILoggerService>();

        SetupMockEmptyReturns();

        _handler = new GetStreetcodeByFilterHandler(
            _mockRepositoryWrapper.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WhenStreetcodeMatches_ReturnsCorrectBranches()
    {
        var request = CreateRequest("test");

        var streetcodes = new List<StreetcodeContent>
        {
            new() { Id = 1, Title = "test-title", Alias = "none", Teaser = "none", TransliterationUrl = "none" },
            new() { Id = 2, Title = "none", Alias = "test-alias", Teaser = "none", TransliterationUrl = "none" },
            new() { Id = 3, Title = "none", Alias = "none", Teaser = "test-teaser", TransliterationUrl = "none" },
            new() { Id = 4, Title = "none", Alias = "none", Teaser = "none", TransliterationUrl = "test-url" }
        };

        MockGetAllAsync(
            _mockRepositoryWrapper.Setup(r => r.StreetcodeRepository.GetAllAsync(
                It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>())),
            streetcodes);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(4);
        result.Value.Should().Contain(x => x.Content == "test-title");
        result.Value.Should().Contain(x => x.Content == "test-alias");
        result.Value.Should().Contain(x => x.Content == "test-teaser");
        result.Value.Should().Contain(x => x.Content == "test-url");
    }

    [Fact]
    public async Task Handle_WhenTextMatches_ReturnsCorrectBranches()
    {
        var request = CreateRequest("word");
        var dummyStreetcode = new StreetcodeContent { Id = 1, TransliterationUrl = "url" };

        var texts = new List<TextEntity>
        {
            new() { Title = "word-title", TextContent = "none", Streetcode = dummyStreetcode },
            new() { Title = "none", TextContent = "word-content", Streetcode = dummyStreetcode }
        };

        MockGetAllAsync(
            _mockRepositoryWrapper.Setup(r => r.TextRepository.GetAllAsync(
                It.IsAny<Expression<Func<TextEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TextEntity>, IIncludableQueryable<TextEntity, object>>>())),
            texts);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.All(x => x.BlockName == "text").Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenFactMatches_ReturnsCorrectBranches()
    {
        var request = CreateRequest("fact");
        var dummyStreetcode = new StreetcodeContent { Id = 1 };

        var facts = new List<FactEntity>
        {
            new() { Title = "fact-title", FactContent = "none", Streetcode = dummyStreetcode },
            new() { Title = "none", FactContent = "fact-content", Streetcode = dummyStreetcode }
        };

        MockGetAllAsync(
            _mockRepositoryWrapper.Setup(r => r.FactRepository.GetAllAsync(
                It.IsAny<Expression<Func<FactEntity, bool>>>(),
                It.IsAny<Func<IQueryable<FactEntity>, IIncludableQueryable<FactEntity, object>>>())),
            facts);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Value.Should().HaveCount(2);
        result.Value.All(x => x.BlockName == "wow-facts").Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenTimelineMatches_ReturnsCorrectBranches()
    {
        var request = CreateRequest("time");
        var dummyStreetcode = new StreetcodeContent { Id = 1 };

        var timelines = new List<TimelineItem>
        {
            new() { Title = "time-title", Description = "none", Streetcode = dummyStreetcode },
            new() { Title = "none", Description = "time-desc", Streetcode = dummyStreetcode }
        };

        MockGetAllAsync(
            _mockRepositoryWrapper.Setup(r => r.TimelineRepository.GetAllAsync(
                It.IsAny<Expression<Func<TimelineItem, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItem>, IIncludableQueryable<TimelineItem, object>>>())),
            timelines);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Value.Should().HaveCount(2);
        result.Value.All(x => x.BlockName == "timeline").Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenArtMatches_ReturnsCorrectBranches_AndSkipsNullStreetcodes()
    {
        var request = CreateRequest("art");

        var validStreetcodeArt = new StreetcodeArt { Streetcode = new StreetcodeContent { Id = 1 } };
        var nullStreetcodeArt = new StreetcodeArt { Streetcode = null };

        var arts = new List<Art>
        {
            new()
            {
                Description = "art-desc",
                StreetcodeArts = new List<StreetcodeArt> { validStreetcodeArt, nullStreetcodeArt }
            }
        };

        MockGetAllAsync(
            _mockRepositoryWrapper.Setup(r => r.ArtRepository.GetAllAsync(
                It.IsAny<Expression<Func<Art, bool>>>(),
                It.IsAny<Func<IQueryable<Art>, IIncludableQueryable<Art, object>>>())),
            arts);

        var result = await _handler.Handle(request, CancellationToken.None);

        result.Value.Should().HaveCount(1);
        result.Value.First().BlockName.Should().Be("art-gallery");
    }

    [Fact]
    public async Task Handle_WhenNothingMatches_ReturnsEmptyList()
    {
        var request = CreateRequest("not-found-query");

        var result = await _handler.Handle(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    private static GetStreetcodeByFilterQuery CreateRequest(string query)
    {
        return new GetStreetcodeByFilterQuery(new StreetcodeFilterRequestDTO { SearchQuery = query });
    }

    private static void MockGetAllAsync<T>(
        Moq.Language.Flow.ISetup<IRepositoryWrapper, Task<IEnumerable<T>>> setup,
        IEnumerable<T> returns) where T : class
    {
        setup.ReturnsAsync(returns);
    }

    private void SetupMockEmptyReturns()
    {
        MockGetAllAsync(
            _mockRepositoryWrapper.Setup(r => r.StreetcodeRepository.GetAllAsync(
                It.IsAny<Expression<Func<StreetcodeContent, bool>>>(),
                It.IsAny<Func<IQueryable<StreetcodeContent>, IIncludableQueryable<StreetcodeContent, object>>>())),
            Enumerable.Empty<StreetcodeContent>());

        MockGetAllAsync(
            _mockRepositoryWrapper.Setup(r => r.TextRepository.GetAllAsync(
                It.IsAny<Expression<Func<TextEntity, bool>>>(),
                It.IsAny<Func<IQueryable<TextEntity>, IIncludableQueryable<TextEntity, object>>>())),
            Enumerable.Empty<TextEntity>());

        MockGetAllAsync(
            _mockRepositoryWrapper.Setup(r => r.FactRepository.GetAllAsync(
                It.IsAny<Expression<Func<FactEntity, bool>>>(),
                It.IsAny<Func<IQueryable<FactEntity>, IIncludableQueryable<FactEntity, object>>>())),
            Enumerable.Empty<FactEntity>());

        MockGetAllAsync(
            _mockRepositoryWrapper.Setup(r => r.TimelineRepository.GetAllAsync(
                It.IsAny<Expression<Func<TimelineItem, bool>>>(),
                It.IsAny<Func<IQueryable<TimelineItem>, IIncludableQueryable<TimelineItem, object>>>())),
            Enumerable.Empty<TimelineItem>());

        MockGetAllAsync(
            _mockRepositoryWrapper.Setup(r => r.ArtRepository.GetAllAsync(
                It.IsAny<Expression<Func<Art, bool>>>(),
                It.IsAny<Func<IQueryable<Art>, IIncludableQueryable<Art, object>>>())),
            Enumerable.Empty<Art>());
    }
}