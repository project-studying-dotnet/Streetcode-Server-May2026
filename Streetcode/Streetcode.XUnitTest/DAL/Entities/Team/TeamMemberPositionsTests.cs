using FluentAssertions;
using Streetcode.DAL.Entities.Team;
using Xunit;

namespace Streetcode.XUnitTest.DAL.Entities.Team;

public class TeamMemberPositionsTests
{
    private const int TeamMemberId = 5;
    private const int PositionsId = 10;

    [Fact]
    public void Properties_ShouldSetValuesCorrectly()
    {
        var entity = new TeamMemberPositions
        {
            TeamMemberId = TeamMemberId,
            PositionsId = PositionsId,
            Positions = null!,
            TeamMember = null!,
        };

        entity.TeamMemberId.Should().Be(TeamMemberId);
        entity.PositionsId.Should().Be(PositionsId);

        entity.Positions.Should().BeNull();
        entity.TeamMember.Should().BeNull();
    }
}
