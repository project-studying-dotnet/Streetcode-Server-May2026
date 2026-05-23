using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.StreetcodePartnersSeeder
{
    [ExcludeFromCodeCoverage]
    internal static class StreetcodePartnersSeeder
    {
        public static async Task FillSeedAsync(
     StreetcodeDbContext context)
        {
            var data = new[]
            {
                new { Sc = 2, P = 1 },
                new { Sc = 2, P = 2 },
                new { Sc = 2, P = 3 },
                new { Sc = 1, P = 1 },
                new { Sc = 1, P = 2 },
                new { Sc = 1, P = 3 }
            };

            var entities = data.Select(item => new StreetcodePartner
            {
                StreetcodeId = item.Sc,
                PartnerId = item.P
            }).ToList();

            await context.StreetcodePartners.SeedIfEmptyAsync(
            entities,
            context);
        }
    }
}
