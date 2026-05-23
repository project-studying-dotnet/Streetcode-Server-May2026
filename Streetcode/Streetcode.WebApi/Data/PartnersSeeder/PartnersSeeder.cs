using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.PartnersSeeder
{
    [ExcludeFromCodeCoverage]
    public static class PartnersSeeder
    {
        private const string SoftServeUrl = "https://" + "www.softserveinc.com/en-us";
        private const string ParimatchUrl = "https://" + "parimatch.com/";
        private const string SalesforceUrl = "https://" + "partners.salesforce.com/pdx/s/?language=en_US&redirected=RGSUDODQUL";
        public static async Task FillSeedAsync(
     StreetcodeDbContext context)
        {
            const string desc1 = "Український культурний фонд є флагманською українською інституцією культури, яка у своїй діяльності інтегрує " +
                                 "різні види мистецтва – від сучасного мистецтва, нової музики й театру до літератури та музейної справи. " +
                                 "Мистецький арсенал є флагманською українською інституцією культури, яка у своїй діяльності інтегрує різні " +
                                 "види мистецтва – від сучасного мистецтва, нової музики й театру до літератури та музейної справи.";

            var data = new[]
            {
                new { IsKey = true, Title = "SoftServe", Desc = desc1, Logo = 12, Url = SoftServeUrl, UrlTitle = (string?)"go to SoftServe page" },
                new { IsKey = false, Title = "Parimatch", Desc = "...", Logo = 13, Url = ParimatchUrl, UrlTitle = (string?)null },
                new { IsKey = false, Title = "comunity partner", Desc = "...", Logo = 14, Url = SalesforceUrl, UrlTitle = (string?)null }
            };

            var entities = data.Select(item => new Partner
            {
                IsKeyPartner = item.IsKey,
                Title = item.Title,
                Description = item.Desc,
                LogoId = item.Logo,
                TargetUrl = item.Url,
                UrlTitle = item.UrlTitle
            }).ToList();

            await context.Partners.SeedIfEmptyAsync(
                entities,
                context);
            }
    }
}
