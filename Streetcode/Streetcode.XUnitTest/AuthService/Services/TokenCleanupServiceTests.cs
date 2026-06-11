using System.Reflection;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Streetcode.Auth.Data;
using Streetcode.Auth.Models.Entities;
using Streetcode.Auth.Services;
using Xunit;

public class TokenCleanupServiceTests
{
    private ApplicationDbContext CreateDb(string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task DoWork_ShouldRemoveExpiredTokens()
    {
        var dbName = Guid.NewGuid().ToString();

        // ARRANGE
        await using (var context = CreateDb(dbName))
        {
            context.RefreshTokens.AddRange(
                new RefreshToken
                {
                    TokenHash = "1",
                    Expires = DateTime.UtcNow.AddDays(-1)
                },
                new RefreshToken
                {
                    TokenHash = "2",
                    Expires = DateTime.UtcNow.AddDays(1)
                });

            await context.SaveChangesAsync();
        }

        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(o =>
            o.UseInMemoryDatabase(dbName));

        var provider = services.BuildServiceProvider();

        var service = new TokenCleanupService(provider);

        var method = typeof(TokenCleanupService)
            .GetMethod("DoWork", BindingFlags.NonPublic | BindingFlags.Instance);

        await (Task)method!.Invoke(service, null)!;

        // ASSERT
        await using var verifyContext = CreateDb(dbName);

        var remaining = await verifyContext.RefreshTokens.ToListAsync();

        remaining.Should().HaveCount(1);
        remaining.First().TokenHash.Should().Be("2");
    }
}