using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.StreetcodePartnersSeeder
{
    [ExcludeFromCodeCoverage]
    internal class StreetcodePartnersSeeder
    {
        public static async Task FillSeedAsync(
     StreetcodeDbContext context)
        {
        var entities = new List<StreetcodePartner>
        {
            new()
            {
                StreetcodeId = 2,
                PartnerId = 1
            },
            new ()
            {
                StreetcodeId = 2,
                PartnerId = 2
            },
            new ()
            {
                StreetcodeId = 2,
                PartnerId = 3
            },
            new ()
            {
                StreetcodeId = 1,
                PartnerId = 1
            },
            new ()
            {
                StreetcodeId = 1,
                PartnerId = 2
            },
            new ()
            {
                StreetcodeId = 1,
                PartnerId = 3
            }
        };

        await context.StreetcodePartners.SeedIfEmptyAsync(
            entities,
            context);
        }
    }
}
