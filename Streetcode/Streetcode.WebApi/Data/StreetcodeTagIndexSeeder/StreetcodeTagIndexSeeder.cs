using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.AdditionalContent;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.StreetcodeTagIndexSeeder
{
    [ExcludeFromCodeCoverage]
    internal static class StreetcodeTagIndexSeeder
    {
        public static async Task FillSeedAsync(
     StreetcodeDbContext context)
        {
            var data = new[]
              {
                (1, 1), (1, 2), (2, 1), (4, 2), (7, 2), (8, 2), (9, 2), (10, 2)
              };

            var entities = data.Select(item => new StreetcodeTagIndex
            {
                TagId = item.Item1,
                StreetcodeId = item.Item2,
                IsVisible = true
            }).ToList();

            await context.StreetcodeTagIndices.SeedIfEmptyAsync(
                entities,
                context);
        }
    }
}
