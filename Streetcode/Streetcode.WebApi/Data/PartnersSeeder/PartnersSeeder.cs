using Streetcode.DAL.Entities.Partners;
using Streetcode.WebApi.InitialData.SeederExtensions;
using Streetcode.DAL.Persistence;

namespace Streetcode.WebApi.InitialData.PartnersSeeder
{
    public class PartnersSeeder
    {
        public static async Task FillSeedAsync(
     StreetcodeDbContext context)
        {
        var entities = new List<Partner>
        {
            new()
            {
                IsKeyPartner = true,
                Title = "SoftServe",
                Description = "Український культурний фонд є флагманською українською інституцією культури, яка у своїй діяльності інтегрує" +
                    " різні види мистецтва – від сучасного мистецтва, нової музики й театру до літератури та музейної справи." +
                    " Мистецький арсенал є флагманською українською інституцією культури, яка у своїй діяльності інтегрує різні" +
                    " види мистецтва – від сучасного мистецтва, нової музики й театру до літератури та музейної справи.",
                LogoId = 12,
                TargetUrl = "https://www.softserveinc.com/en-us",
                UrlTitle = "go to SoftServe page"
            },
            new Partner
            {
                Title = "Parimatch",
                Description = "Конторка для лошків з казіничами та лохотроном, аби стягнути побільше бабок з довірливих дурбобиків",
                LogoId = 13,
                TargetUrl = "https://parimatch.com/"
            },
            new Partner
            {
                Title = "comunity partner",
                Description = "Класна платформа, я зацінив, а ти?",
                LogoId = 14,
                TargetUrl = "https://partners.salesforce.com/pdx/s/?language=en_US&redirected=RGSUDODQUL"
            }
        };

        await context.Partners.SeedIfEmptyAsync(
            entities,
            context);
        }
    }
}
