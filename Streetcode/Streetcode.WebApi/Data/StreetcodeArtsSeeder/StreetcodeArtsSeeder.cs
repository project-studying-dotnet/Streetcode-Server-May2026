using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.StreetcodeArtsSeeder
{
    [ExcludeFromCodeCoverage]
    internal static class StreetcodeArtsSeeder
    {
        public static async Task FillSeedAsync(
     StreetcodeDbContext context)
        {
            var data = new[]
              {
                (1, 1, 1), (2, 1, 2), (3, 1, 3), (4, 1, 4), (5, 1, 5), (6, 1, 6),
                (7, 2, 1), (4, 2, 2), (5, 2, 3), (6, 2, 4)
              };

            var entities = data.Select(item => new StreetcodeArt
            {
                ArtId = item.Item1,
                StreetcodeId = item.Item2,
                Index = item.Item3
            }).ToList();

            await context.StreetcodeArts.SeedIfEmptyAsync(
                entities,
                context);
        }
    }
}
