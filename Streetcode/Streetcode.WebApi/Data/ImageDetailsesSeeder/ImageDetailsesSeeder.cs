using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.ImageDetailsesSeeder
{
    [ExcludeFromCodeCoverage]
    public class ImageDetailsesSeeder
    {
        public static async Task FillSeedAsync(
       StreetcodeDbContext context)
        {
            var entities = new List<ImageDetails>
        {
            new()
            {
                ImageId = 6,
                Alt = "Additional inforamtaion for  wow-fact photo 1"
            },
            new ()
            {
                ImageId = 16,
                Alt = "Additional inforamtaion for  wow-fact photo 2"
            },
            new ()
            {
                ImageId = 17,
                Alt = "Additional inforamtaion for  wow-fact photo 3"
            },
            new ()
            {
                ImageId = 19,
                Alt = "Additional inforamtaion for  wow-fact photo 3"
            }
        };

            await context.ImageDetailses.SeedIfEmptyAsync(
                entities,
                context);
        }
    }
}
