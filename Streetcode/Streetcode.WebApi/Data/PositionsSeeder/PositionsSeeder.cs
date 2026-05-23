using Streetcode.DAL.Entities.Team;
using Streetcode.WebApi.InitialData.SeederExtensions;
using Streetcode.DAL.Persistence;

namespace Streetcode.WebApi.InitialData.PositionsSeeder
{
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
