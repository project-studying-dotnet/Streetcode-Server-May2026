using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.StreetcodeArtsSeeder
{
    [ExcludeFromCodeCoverage]
    internal class StreetcodeArtsSeeder
    {
        public static async Task FillSeedAsync(
     StreetcodeDbContext context)
        {
            var entities = new List<StreetcodeArt>
        {
            new()
            {
                ArtId = 1,
                StreetcodeId = 1,
                Index = 1,
            },
            new ()
            {
                ArtId = 2,
                StreetcodeId = 1,
                Index = 2,
            },
            new ()
            {
                ArtId = 3,
                StreetcodeId = 1,
                Index = 3,
            },
            new ()
            {
                ArtId = 4,
                StreetcodeId = 1,
                Index = 4,
            },
            new ()
            {
                ArtId = 5,
                StreetcodeId = 1,
                Index = 5,
            },
            new ()
            {
                ArtId = 6,
                StreetcodeId = 1,
                Index = 6,
            },
            new ()
            {
                ArtId = 7,
                StreetcodeId = 2,
                Index = 1,
            },
            new ()
            {
                ArtId = 4,
                StreetcodeId = 2,
                Index = 2,
            },
            new ()
            {
                ArtId = 5,
                StreetcodeId = 2,
                Index = 3,
            },
            new ()
            {
                ArtId = 6,
                StreetcodeId = 2,
                Index = 4,
            }
        };

            await context.StreetcodeArts.SeedIfEmptyAsync(
                entities,
                context);
        }
    }
}
