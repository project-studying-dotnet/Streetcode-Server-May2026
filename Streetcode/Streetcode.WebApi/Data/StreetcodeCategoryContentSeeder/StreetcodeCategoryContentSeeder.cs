using Streetcode.DAL.Entities.Sources;
using Streetcode.WebApi.InitialData.SeederExtensions;
using Streetcode.DAL.Persistence;

namespace Streetcode.WebApi.InitialData.StreetcodeCategoryContentSeeder
{
    public class StreetcodeCategoryContentSeeder
    {
        public static async Task FillSeedAsync(
     StreetcodeDbContext context)
        {
            var entities = new List<StreetcodeCategoryContent>
        {
            new()
            {
                Text = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
                SourceLinkCategoryId = 1,
                StreetcodeId = 2
            },
            new ()
            {
                Text = "Хроніки про Т. Г. Шевченко",
                SourceLinkCategoryId = 2,
                StreetcodeId = 2
            },
            new ()
            {
                Text = "Цитати про Шевченка",
                SourceLinkCategoryId = 3,
                StreetcodeId = 2
            },
            new ()
            {
                Text = "Пряма мова",
                SourceLinkCategoryId = 3,
                StreetcodeId = 1
            }
        };

            await context.StreetcodeCategoryContent.SeedIfEmptyAsync(
                entities,
                context);
        }
    }
}
