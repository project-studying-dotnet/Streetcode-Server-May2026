using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.PositionsSeeder
{
    [ExcludeFromCodeCoverage]
    public class PositionsSeeder
    {
        public static async Task FillSeedAsync(
        StreetcodeDbContext context)
        {
        var entities = new List<Positions>
        {
            new()
            {
             Position = "Голова і засновниця ГО"
            }
        };

        await context.Positions.SeedIfEmptyAsync(
            entities,
            context);
        }
    }
}
