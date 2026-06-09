using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.TeamMemberPositionSeeder
{
    [ExcludeFromCodeCoverage]
    public static class TeamMemberPositionSeeder
    {
        public static async Task FillSeedAsync(
        StreetcodeDbContext context)
        {
            var data = new[]
            {
                new { Pos = 1, Member = 1 },
                new { Pos = 1, Member = 2 },
                new { Pos = 1, Member = 3 }
            };

            var entities = data.Select(item => new TeamMemberPositions
            {
                PositionsId = item.Pos,
                TeamMemberId = item.Member
            }).ToList();

            await context.TeamMemberPosition.SeedIfEmptyAsync(
            entities,
            context);
        }
    }
}
