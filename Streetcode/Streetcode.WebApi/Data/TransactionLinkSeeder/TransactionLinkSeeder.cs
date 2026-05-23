using Streetcode.DAL.Entities.Transactions;
using Streetcode.WebApi.InitialData.SeederExtensions;
using Streetcode.DAL.Persistence;

namespace Streetcode.WebApi.InitialData.TransactionLinkSeeder
{
    public class TransactionLinkSeeder
    {
        public static async Task FillSeedAsync(
     StreetcodeDbContext context)
        {
            var entities = new List<TransactionLink>
        {
            new()
            {
                Url = "https://streetcode/1",
                StreetcodeId = 1
            },
            new ()
            {
                Url = "https://streetcode/2",
                StreetcodeId = 2
            }
        };

            await context.TransactionLinks.SeedIfEmptyAsync(
                entities,
                context);
        }
    }
}
