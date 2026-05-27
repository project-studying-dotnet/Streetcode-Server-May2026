using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.ImageDetailsesSeeder
{
    [ExcludeFromCodeCoverage]
    public static class ImageDetailsesSeeder
    {
        public static async Task FillSeedAsync(
       StreetcodeDbContext context)
        {
            var data = new[]
            {
                new { Id = 6, Alt = "Additional inforamtaion for  wow-fact photo 1" },
                new { Id = 16, Alt = "Additional inforamtaion for  wow-fact photo 2" },
                new { Id = 17, Alt = "Additional inforamtaion for  wow-fact photo 3" },
                new { Id = 19, Alt = "Additional inforamtaion for  wow-fact photo 3" }
            };

            var entities = data.Select(item => new ImageDetails
            {
                ImageId = item.Id,
                Alt = item.Alt
            }).ToList();

            await context.ImageDetailses.SeedIfEmptyAsync(
                entities,
                context);
        }
    }
}
