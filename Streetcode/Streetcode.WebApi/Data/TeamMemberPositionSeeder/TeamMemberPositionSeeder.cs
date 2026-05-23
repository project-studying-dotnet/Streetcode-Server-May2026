using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.TeamMemberPositionSeeder
{
    [ExcludeFromCodeCoverage]
    public class TeamMemberPositionSeeder
    {
        public static async Task FillSeedAsync(
        StreetcodeDbContext context)
        {
        var entities = new List<TeamMemberPositions>
        {
            new()
            {
                PositionsId = 1,
                TeamMemberId = 1
            },
            new ()
            {
                PositionsId = 1,
                TeamMemberId = 2
            },
            new ()
            {
                PositionsId = 1,
                TeamMemberId = 3
            }
        };

        await context.TeamMemberPosition.SeedIfEmptyAsync(
            entities,
            context);
        }
    }
}
