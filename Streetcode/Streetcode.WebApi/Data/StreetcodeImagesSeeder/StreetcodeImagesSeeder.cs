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
            var data = new[]
             {
                new { Img = 1, Sc = 1 },
                new { Img = 5, Sc = 1 },
                new { Img = 1, Sc = 2 },
                new { Img = 23, Sc = 2 }
             };

            var entities = data.Select(item => new StreetcodeImage
            {
                ImageId = item.Img,
                StreetcodeId = item.Sc
            }).ToList();

            await context.StreetcodeImages.SeedIfEmptyAsync(
                entities,
                context);
        }
    }
}
