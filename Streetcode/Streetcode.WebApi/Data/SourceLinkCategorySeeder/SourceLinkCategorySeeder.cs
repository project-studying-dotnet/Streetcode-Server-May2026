using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.SourceLinkCategorySeeder
{
    [ExcludeFromCodeCoverage]
    public static class SourceLinkCategorySeeder
    {
        public static async Task FillSeedAsync(
       StreetcodeDbContext context)
        {
            var data = new[]
            {
                new { T = "Книги", Img = 9 },
                new { T = "Фільми", Img = 10 },
                new { T = "Цитати", Img = 11 }
            };

            var entities = data.Select(item => new SourceLinkCategory
            {
                Title = item.T,
                ImageId = item.Img
            }).ToList();

            await context.SourceLinks.SeedIfEmptyAsync(
                entities,
                context);
        }
    }
}
