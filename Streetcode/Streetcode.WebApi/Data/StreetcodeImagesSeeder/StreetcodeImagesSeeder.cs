using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.StreetcodeImagesSeeder
{
    [ExcludeFromCodeCoverage]
    internal class StreetcodeImagesSeeder
    {
        public static async Task FillSeedAsync(
     StreetcodeDbContext context)
        {
            var entities = new List<StreetcodeImage>
        {
            new()
            {
                ImageId = 1,
                StreetcodeId = 1,
            },
            new ()
            {
                ImageId = 5,
                StreetcodeId = 1,
            },
            new ()
            {
                ImageId = 1,
                StreetcodeId = 2,
            },
            new ()
            {
                ImageId = 23,
                StreetcodeId = 2,
            }
        };

            await context.StreetcodeImages.SeedIfEmptyAsync(
                entities,
                context);
        }
    }
}
