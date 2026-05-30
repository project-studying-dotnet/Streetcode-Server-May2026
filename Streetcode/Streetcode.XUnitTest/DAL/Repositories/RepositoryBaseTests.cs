using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.News;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Xunit;

namespace Streetcode.XUnitTest.DAL.Repositories;

public class RepositoryBaseTests
{
    [Fact]
    public void Include_ShouldReturnDbSet_WhenNoIncludesProvided()
    {
        using var context = CreateContext();
        var repository = new TestNewsRepository(context);

        context.News.Add(CreateNews(1, "url-1"));
        context.SaveChanges();

        var result = repository.Include().ToList();

        result.Should().ContainSingle();
    }

    [Fact]
    public void Include_ShouldApplySingleInclude_WhenIncludeProvided()
    {
        using var context = CreateContext();
        var repository = new TestNewsRepository(context);

        context.News.Add(CreateNews(1, "url-1"));
        context.SaveChanges();

        var result = repository
            .Include(news => news.Image!)
            .ToList();

        result.Should().ContainSingle();
        result[0].Image.Should().NotBeNull();
    }

    [Fact]
    public async Task GetFirstOrDefaultAsync_ShouldReturnEntity_WhenPredicateMatches()
    {
        using var context = CreateContext();
        var repository = new TestNewsRepository(context);

        context.News.Add(CreateNews(1, "url-1"));
        await context.SaveChangesAsync();

        var result = await repository.GetFirstOrDefaultAsync(
            news => news.URL == "url-1");

        result.Should().NotBeNull();
        result!.URL.Should().Be("url-1");
    }

    [Fact]
    public async Task CreateAsync_ShouldAddEntity()
    {
        using var context = CreateContext();
        var repository = new TestNewsRepository(context);

        var news = CreateNews(1, "url-1");

        await repository.CreateAsync(news);
        await context.SaveChangesAsync();

        context.News.Should().ContainSingle();
    }

    private static StreetcodeDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<StreetcodeDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new StreetcodeDbContext(options);
    }

    private static News CreateNews(int id, string url) =>
        new()
        {
            Id = id,
            URL = url,
            Title = $"Title {id}",
            Text = $"Text {id}",
            Image = new Image
            {
                Id = id,
                BlobName = $"image-{id}.jpg",
                MimeType = "image/jpeg",
            },
        };

    private sealed class TestNewsRepository : RepositoryBase<News>
    {
        public TestNewsRepository(StreetcodeDbContext context)
            : base(context)
        {
        }
    }
}