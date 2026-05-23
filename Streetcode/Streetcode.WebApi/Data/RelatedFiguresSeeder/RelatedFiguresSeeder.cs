using Streetcode.DAL.Entities.Streetcode;
using Streetcode.WebApi.InitialData.SeederExtensions;
using Streetcode.DAL.Persistence;

namespace Streetcode.WebApi.InitialData.RelatedFiguresSeeder
{
    public class RelatedFiguresSeeder
    {
        public static async Task FillSeedAsync(
     StreetcodeDbContext context)
        {
            var entities = new List<RelatedFigure>
        {
            new()
            {
                ObserverId = 2,
                TargetId = 1
            },
            new ()
            {
                ObserverId = 1,
                TargetId = 2
            }
        };

            await context.RelatedFigures.SeedIfEmptyAsync(
                entities,
                context);
        }
    }
}
