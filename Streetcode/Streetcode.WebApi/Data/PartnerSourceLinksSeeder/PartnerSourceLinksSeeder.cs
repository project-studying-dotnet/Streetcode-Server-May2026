using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.PartnerSourceLinksSeeder
{
    [ExcludeFromCodeCoverage]
    internal class PartnerSourceLinksSeeder
    {
        public static async Task FillSeedAsync(
     StreetcodeDbContext context)
        {
            var data = new[]
            {
                new { Type = LogoType.Twitter, Url = "https://twitter.com/SoftServeInc", PId = 1 },
                new { Type = LogoType.Instagram, Url = "https://www.instagram.com/softserve_people/", PId = 1 },
                new { Type = LogoType.Facebook, Url = "https://www.facebook.com/SoftServeCompany", PId = 1 }
            };

            var entities = data.Select(item => new PartnerSourceLink
            {
                LogoType = item.Type,
                TargetUrl = item.Url,
                PartnerId = item.PId
            }).ToList();

            await context.PartnerSourceLinks.SeedIfEmptyAsync(
            entities,
            context);
        }
    }
}
