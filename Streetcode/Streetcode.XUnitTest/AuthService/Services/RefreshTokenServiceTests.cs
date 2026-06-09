using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Streetcode.Auth.Data;
using Streetcode.Auth.Models.Entities;
using Streetcode.Auth.Services.Users;
using Xunit;

public class RefreshTokenServiceTests
{
    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    private static RefreshTokenService CreateService(ApplicationDbContext context)
        => new RefreshTokenService(context);

    [Fact]
    public void Generate_ShouldReturnToken()
    {
        var service = CreateService(CreateDbContext());

        var token = service.Generate();

        token.Should().NotBeNullOrEmpty();
        Convert.FromBase64String(token).Length.Should().Be(64);
    }

    [Fact]
    public async Task SaveAsync_ShouldStoreToken()
    {
        var context = CreateDbContext();
        var service = CreateService(context);

        var token = service.Generate();

        await service.SaveAsync(1, token);

        var saved = await context.RefreshTokens.FirstAsync();

        saved.UserId.Should().Be(1);
        saved.IsUsed.Should().BeFalse();
        saved.IsRevoked.Should().BeFalse();
        saved.TokenHash.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RefreshAsync_ShouldRotateToken()
    {
        var context = CreateDbContext();
        var service = CreateService(context);

        var oldToken = "stable-token";

        var testUser = new User { Id = 1, Name = "testuser", Surname = "testuser" };
        context.Users.Add(testUser);
        await context.SaveChangesAsync();

        // SAVE
        await service.SaveAsync(1, oldToken);

        // FORCE FLUSH (важно для InMemory)
        //await context.SaveChangesAsync();

        // ACT
        var result = await service.RefreshAsync(oldToken);

        result.user.Should().NotBeNull();
        result.newRefreshToken.Should().NotBeNullOrEmpty();

        var tokens = await context.RefreshTokens.ToListAsync();

        tokens.Should().HaveCount(2);

        var used = tokens.Single(x => x.IsUsed && x.IsRevoked);
        var fresh = tokens.Single(x => !x.IsUsed && !x.IsRevoked);

        used.UserId.Should().Be(1);
        fresh.UserId.Should().Be(1);
    }

    [Fact]
    public async Task RefreshAsync_InvalidToken_ShouldThrow()
    {
        var context = CreateDbContext();
        var service = CreateService(context);

        var testUser = new User { Id = 1, Name = "testuser", Surname = "testuser" };
        context.Users.Add(testUser);
        await context.SaveChangesAsync();

        var oldToken = "stable-token";
        await service.SaveAsync(1, oldToken);

        var invalidToken = "invalid-token";
        Func<Task> act = async () =>
            await service.RefreshAsync(invalidToken);

        await act.Should()
            .ThrowAsync<SecurityTokenException>()
            .WithMessage("Invalid refresh token");
    }

    [Fact]
    public async Task RevokeAsync_ShouldMarkTokenAsRevoked()
    {
        var context = CreateDbContext();
        var service = CreateService(context);

        var token = service.Generate();

        await service.SaveAsync(1, token);

        await service.RevokeAsync(token);

        var saved = await context.RefreshTokens.FirstAsync();

        saved.IsRevoked.Should().BeTrue();
    }
}