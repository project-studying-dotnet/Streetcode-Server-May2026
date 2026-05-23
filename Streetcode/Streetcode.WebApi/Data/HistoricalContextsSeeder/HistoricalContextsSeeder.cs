using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.HistoricalContextsSeeder
{
    [ExcludeFromCodeCoverage]
    public class HistoricalContextsSeeder
    {
        public static async Task FillSeedAsync(
     StreetcodeDbContext context)
        {
            var titles = new[]
            {
                "Дитинство",
                "Студентство",
                "Життя в Петербурзі",
                "Незалежна Україна",
                "Революція гідності",
                "Збройна агресія Росії",
                "Повномасштабне вторгнення Росії"
            };

            var entities = titles.Select(title => new HistoricalContext
            {
                Title = title
            }).ToList();

            await context.HistoricalContexts.SeedIfEmptyAsync(
            entities,
            context);
        }
    }
}
