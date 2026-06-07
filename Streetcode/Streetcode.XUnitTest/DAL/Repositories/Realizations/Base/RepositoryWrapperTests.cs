using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Xunit;
using FluentAssertions;

namespace Streetcode.XUnitTest.DAL.Repositories.Base;

public class RepositoryWrapperTests
{
    private readonly DbContextOptions<StreetcodeDbContext> _options;

    public RepositoryWrapperTests()
    {
        _options = new DbContextOptionsBuilder<StreetcodeDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void CommentRepository_ShouldInitializeLazilyAndReturnSameInstance()
    {
        // Arrange
        using var context = new StreetcodeDbContext(_options);
        var wrapper = new RepositoryWrapper(context);

        // Act
        var repoFirstCall = wrapper.CommentRepository;
        var repoSecondCall = wrapper.CommentRepository;

        // Assert
        repoFirstCall.Should().NotBeNull();
        repoSecondCall.Should().NotBeNull();
        repoFirstCall.Should().BeSameAs(repoSecondCall);
    }
}
