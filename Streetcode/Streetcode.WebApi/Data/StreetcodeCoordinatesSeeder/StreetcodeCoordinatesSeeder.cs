using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.StreetcodeCoordinatesSeeder
{
    [ExcludeFromCodeCoverage]
    public static class StreetcodeCoordinatesSeeder
    {
        public static async Task FillSeedAsync(
        StreetcodeDbContext context)
        {
            var entities = new List<StreetcodeCoordinate>
        {
            new()
            {
                Latitude = 49.8429M,
                Longtitude = 24.0311M,
                StreetcodeId = 1
            },
            new ()
            {
                Latitude = 50.4550M,
                Longtitude = 30.5238M,
                StreetcodeId = 2
            }
        };

            await context.StreetcodeCoordinates.SeedIfEmptyAsync(
                entities,
                context);
        }
    }
}
