using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.RelatedTerms
{
    [ExcludeFromCodeCoverage]
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
