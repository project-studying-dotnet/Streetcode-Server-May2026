using Streetcode.DAL.Entities.Timeline;
using Streetcode.WebApi.InitialData.SeederExtensions;
using Streetcode.DAL.Persistence;

namespace Streetcode.WebApi.InitialData.HistoricalContextsSeeder
{
    public class HistoricalContextsSeeder
    {
        public static async Task FillSeedAsync(
     StreetcodeDbContext context)
        {
        var entities = new List<HistoricalContext>
        {
            new()
            {
                Title = "Дитинство"
            },
            new HistoricalContext
            {
                Title = "Студентство"
            },
            new HistoricalContext
            {
                Title = "Життя в Петербурзі"
            },
            new HistoricalContext
            {
                Title = "Незалежна Україна"
            },
            new HistoricalContext
            {
                Title = "Революція гідності"
            },
            new HistoricalContext
            {
                Title = "Збройна агресія Росії"
            },
            new HistoricalContext
            {
                Title = "Повномасштабне вторгнення Росії"
            }
        };

        await context.HistoricalContexts.SeedIfEmptyAsync(
            entities,
            context);
        }
    }
}
