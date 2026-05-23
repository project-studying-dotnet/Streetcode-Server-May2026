using Streetcode.DAL.Entities.Sources;
using Streetcode.WebApi.InitialData.SeederExtensions;
using Streetcode.DAL.Persistence;

namespace Streetcode.WebApi.InitialData.SourceLinkCategorySeeder
{
    public class SourceLinkCategorySeeder
    {
        public static async Task FillSeedAsync(
       StreetcodeDbContext context)
        {
            var entities = new List<SourceLinkCategory>
        {
            new()
            {
                Title = "Книги",
                ImageId = 9,
            },
            new ()
            {
                Title = "Фільми",
                ImageId = 10,
            },
            new ()
            {
                Title = "Цитати",
                ImageId = 11,
            }
        };

            await context.SourceLinks.SeedIfEmptyAsync(
                entities,
                context);
        }
    }
}
