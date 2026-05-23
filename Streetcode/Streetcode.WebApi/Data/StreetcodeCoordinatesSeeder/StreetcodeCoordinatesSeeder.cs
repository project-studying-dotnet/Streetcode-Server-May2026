using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.WebApi.InitialData.SeederExtensions;
using Streetcode.DAL.Persistence;

namespace Streetcode.WebApi.InitialData.StreetcodeCoordinatesSeeder
{
    public class StreetcodeCoordinatesSeeder
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
