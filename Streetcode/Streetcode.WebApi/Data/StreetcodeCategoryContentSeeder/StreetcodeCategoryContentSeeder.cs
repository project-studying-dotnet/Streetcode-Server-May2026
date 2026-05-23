using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Sources;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.StreetcodeCategoryContentSeeder
{
    [ExcludeFromCodeCoverage]
    public class StreetcodeCategoryContentSeeder
    {
        public static async Task FillSeedAsync(
     StreetcodeDbContext context)
        {
            const string lorem = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.";
            var data = new[]
            {
                new { Txt = lorem, CatId = 1, ScId = 2 },
                new { Txt = "Хроніки про Т. Г. Шевченко", CatId = 2, ScId = 2 },
                new { Txt = "Цитати про Шевченка", CatId = 3, ScId = 2 },
                new { Txt = "Пряма мова", CatId = 3, ScId = 1 }
            };

            var entities = data.Select(item => new StreetcodeCategoryContent
            {
                Text = item.Txt,
                SourceLinkCategoryId = item.CatId,
                StreetcodeId = item.ScId
            }).ToList();

            await context.StreetcodeCategoryContent.SeedIfEmptyAsync(
                entities,
                context);
        }
    }
}
