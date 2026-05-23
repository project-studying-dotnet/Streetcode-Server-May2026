using Streetcode.DAL.Entities.Timeline;
using Streetcode.WebApi.InitialData.SeederExtensions;
using Streetcode.DAL.Persistence;

namespace Streetcode.WebApi.InitialData.HistoricalContextsTimelinesSeeder
{
    public class HistoricalContextsTimelinesSeeder
    {
        public static async Task FillSeedAsync(
     StreetcodeDbContext context)
        {
            var entities = new List<HistoricalContextTimeline>
        {
            new()
            {
                HistoricalContextId = 3,
                TimelineId = 1
            },
            new ()
            {
                HistoricalContextId = 2,
                TimelineId = 2,
            },
            new ()
            {
                HistoricalContextId = 3,
                TimelineId = 3
            },
            new ()
            {
                HistoricalContextId = 3,
                TimelineId = 4
            },
            new ()
            {
                HistoricalContextId = 3,
                TimelineId = 5
            },
            new ()
            {
                HistoricalContextId = 3,
                TimelineId = 6
            },
            new ()
            {
                HistoricalContextId = 3,
                TimelineId = 7
            },
            new ()
            {
                HistoricalContextId = 4,
                TimelineId = 8
            },
            new ()
            {
                HistoricalContextId = 4,
                TimelineId = 9
            },
            new ()
            {
                HistoricalContextId = 5,
                TimelineId = 10
            },
            new ()
            {
                HistoricalContextId = 5,
                TimelineId = 11
            },
            new ()
            {
                HistoricalContextId = 6,
                TimelineId = 12
            },
            new ()
            {
                HistoricalContextId = 6,
                TimelineId = 13
            },
            new ()
            {
                HistoricalContextId = 6,
                TimelineId = 14
            },
            new ()
            {
                HistoricalContextId = 6,
                TimelineId = 15
            },
            new ()
            {
                HistoricalContextId = 6,
                TimelineId = 16
            },
            new ()
            {
                HistoricalContextId = 6,
                TimelineId = 17
            },
            new ()
            {
                HistoricalContextId = 6,
                TimelineId = 18
            },
            new ()
            {
                HistoricalContextId = 7,
                TimelineId = 19
            },
            new ()
            {
                HistoricalContextId = 7,
                TimelineId = 20
            },
            new ()
            {
                HistoricalContextId = 7,
                TimelineId = 21
            },
            new ()
            {
                HistoricalContextId = 7,
                TimelineId = 22
            },
            new ()
            {
                HistoricalContextId = 7,
                TimelineId = 23
            },
            new ()
            {
                HistoricalContextId = 7,
                TimelineId = 24
            }
        };

            await context.HistoricalContextsTimelines.SeedIfEmptyAsync(
                entities,
                context);
        }
    }
}
