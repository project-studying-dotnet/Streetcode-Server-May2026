using Streetcode.DAL.Entities.AdditionalContent;
using Streetcode.WebApi.InitialData.SeederExtensions;
using Streetcode.DAL.Persistence;

namespace Streetcode.WebApi.InitialData.StreetcodeTagIndexSeeder
{
    internal class StreetcodeTagIndexSeeder
    {
        public static async Task FillSeedAsync(
     StreetcodeDbContext context)
        {
            var entities = new List<StreetcodeTagIndex>
        {
            new()
            {
                TagId = 1,
                StreetcodeId = 1,
                IsVisible = true,
            },
            new ()
            {
                TagId = 1,
                StreetcodeId = 2,
                IsVisible = true,
            },
            new ()
            {
                TagId = 2,
                StreetcodeId = 1,
                IsVisible = true,
            },
            new ()
            {
                TagId = 4,
                StreetcodeId = 2,
                IsVisible = true,
            },
            new ()
            {
                TagId = 7,
                StreetcodeId = 2,
                IsVisible = true,
            },
            new ()
            {
                TagId = 8,
                StreetcodeId = 2,
                IsVisible = true,
            },
            new ()
            {
                TagId = 9,
                StreetcodeId = 2,
                IsVisible = true,
            },
            new ()
            {
                TagId = 10,
                StreetcodeId = 2,
                IsVisible = true,
            }
        };

            await context.StreetcodeTagIndices.SeedIfEmptyAsync(
                entities,
                context);
        }
    }
}
