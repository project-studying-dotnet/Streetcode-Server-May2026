using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.AdditionalContent;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.TagsSeeder
{
    [ExcludeFromCodeCoverage]
    internal static class TagsSeeder
    {
        public static async Task FillSeedAsync(
     StreetcodeDbContext context)
        {
            var titles = new[]
            {
                "writer",
                "artist",
                "composer",
                "victory",
                "Наукова школа",
                "Історія",
                "Політика",
                "Активіст",
                "Борці за незалежність",
                "Герої"
            };

            var entities = titles.Select(t => new Tag
            {
                Title = t
            }).ToList();

            await context.Tags.SeedIfEmptyAsync(
                entities,
                context);
        }
    }
}
