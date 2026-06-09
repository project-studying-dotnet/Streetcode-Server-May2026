using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Streetcode.Auth.Data;
using Streetcode.Auth.Models.Entities;
using Streetcode.Auth.Services;
using Xunit;

public class TokenCleanupServiceTests
{
    private ApplicationDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private TokenCleanupService CreateService(ApplicationDbContext context)
    {
        var services = new ServiceCollection();

        services.AddSingleton(context);
        var provider = services.BuildServiceProvider();

        return new TokenCleanupService(provider);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldRemoveExpiredTokens()
    {
        // Arrange
        var context = CreateDb();
        context.RefreshTokens.AddRange(
            new RefreshToken { TokenHash = "1", Expires = DateTime.UtcNow.AddDays(-1) },
            new RefreshToken { TokenHash = "2", Expires = DateTime.UtcNow.AddDays(1) }
        );
        await context.SaveChangesAsync();

        var service = CreateService(context);
        using var cts = new CancellationTokenSource();

        // Act
        var runningTask = service.StartAsync(cts.Token);

        await Task.Delay(200);

        await cts.CancelAsync();
        await runningTask;

        var remaining = await context.RefreshTokens.ToListAsync();
        remaining.Should().HaveCount(1);
        remaining.First().TokenHash.Should().Be("2");
    }
}