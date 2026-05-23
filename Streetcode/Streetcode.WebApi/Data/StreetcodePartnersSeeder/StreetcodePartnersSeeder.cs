using Streetcode.DAL.Entities.Partners;
using Streetcode.WebApi.InitialData.SeederExtensions;
using Streetcode.DAL.Persistence;

namespace Streetcode.WebApi.InitialData.StreetcodePartnersSeeder
{
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
