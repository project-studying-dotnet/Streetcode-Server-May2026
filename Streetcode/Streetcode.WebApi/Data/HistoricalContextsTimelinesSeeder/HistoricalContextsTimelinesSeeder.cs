using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.HistoricalContextsTimelinesSeeder
{
    [ExcludeFromCodeCoverage]
    public class HistoricalContextsTimelinesSeeder
    {
        public static async Task FillSeedAsync(
     StreetcodeDbContext context)
        {
            var data = new[]
              {
                (3, 1), (2, 2), (3, 3), (3, 4), (3, 5), (3, 6), (3, 7),
                (4, 8), (4, 9), (5, 10), (5, 11), (6, 12), (6, 13), (6, 14),
                (6, 15), (6, 16), (6, 17), (6, 18), (7, 19), (7, 20), (7, 21),
                (7, 22), (7, 23), (7, 24)
              };

            var entities = data.Select(item => new HistoricalContextTimeline
            {
                HistoricalContextId = item.Item1,
                TimelineId = item.Item2
            }).ToList();

            await context.HistoricalContextsTimelines.SeedIfEmptyAsync(
                entities,
                context);
        }
    }
}
