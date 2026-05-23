using Streetcode.DAL.Entities.Team;
using Streetcode.WebApi.InitialData.SeederExtensions;
using Streetcode.DAL.Persistence;

namespace Streetcode.WebApi.InitialData.TeamMemberPositionSeeder
{
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
