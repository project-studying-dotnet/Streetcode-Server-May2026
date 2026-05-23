using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.WebApi.InitialData.SeederExtensions;
using Streetcode.DAL.Persistence;

namespace Streetcode.WebApi.InitialData.RelatedTerms
{
    public static class RelatedTerms
    {
        public static async Task FillSeedAsync(
        StreetcodeDbContext context)
        {
        var entities = new List<RelatedTerm>
        {
            new()
            {
                Word = "кріпаків",
                TermId = 3,
            }
        };

        await context.RelatedTerms.SeedIfEmptyAsync(
            entities,
            context);
        }
    }
}
