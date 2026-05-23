using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.RelatedFiguresSeeder
{
    [ExcludeFromCodeCoverage]
    public static class RelatedFiguresSeeder
    {
        public static async Task FillSeedAsync(
     StreetcodeDbContext context)
        {
            var data = new[]
              {
                new { Observer = 2, Target = 1 },
                new { Observer = 1, Target = 2 }
              };

            var entities = data.Select(item => new RelatedFigure
            {
                ObserverId = item.Observer,
                TargetId = item.Target
            }).ToList();

            await context.RelatedFigures.SeedIfEmptyAsync(
                entities,
                context);
        }
    }
}
