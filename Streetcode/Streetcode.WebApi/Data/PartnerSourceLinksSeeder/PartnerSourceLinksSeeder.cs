using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Enums;
using Streetcode.WebApi.InitialData.SeederExtensions;
using Streetcode.DAL.Persistence;

namespace Streetcode.WebApi.InitialData.PartnerSourceLinksSeeder
{
    internal class PartnerSourceLinksSeeder
    {
        public static async Task FillSeedAsync(
     StreetcodeDbContext context)
        {
        var entities = new List<PartnerSourceLink>
        {
            new()
            {
                LogoType = LogoType.Twitter,
                TargetUrl = "https://twitter.com/SoftServeInc",
                PartnerId = 1
            },
            new ()
            {
                LogoType = LogoType.Instagram,
                TargetUrl = "https://www.instagram.com/softserve_people/",
                PartnerId = 1
            },
            new ()
            {
                LogoType = LogoType.Facebook,
                TargetUrl = "https://www.facebook.com/SoftServeCompany",
                PartnerId = 1
            }
        };

        await context.PartnerSourceLinks.SeedIfEmptyAsync(
            entities,
            context);
        }
    }
}
