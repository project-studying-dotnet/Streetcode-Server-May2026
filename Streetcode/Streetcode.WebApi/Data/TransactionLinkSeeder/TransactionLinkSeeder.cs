using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Transactions;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.TransactionLinkSeeder
{
    [ExcludeFromCodeCoverage]
    public static class TransactionLinkSeeder
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
