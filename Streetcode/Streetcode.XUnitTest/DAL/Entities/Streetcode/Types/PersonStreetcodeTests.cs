using FluentAssertions;
using Streetcode.DAL.Entities.Streetcode.Types;
using Xunit;

namespace Streetcode.XUnitTest.DAL.Entities.Streetcode.Types;

public class PersonStreetcodeTests
{
    private const string FirstName = "Taras";
    private const string LastName = "Shevchenko";
    private const string Rank = "Writer";

    [Fact]
    public void Properties_ShouldSetValuesCorrectly()
    {
        var entity = new PersonStreetcode
        {
            FirstName = FirstName,
            LastName = LastName,
            Rank = Rank,
        };

        entity.FirstName.Should().Be(FirstName);
        entity.LastName.Should().Be(LastName);
        entity.Rank.Should().Be(Rank);
    }

    [Fact]
    public void Rank_ShouldAllowNull()
    {
        var entity = new PersonStreetcode
        {
            FirstName = FirstName,
            LastName = LastName,
            Rank = null,
        };

        entity.Rank.Should().BeNull();
    }
}
