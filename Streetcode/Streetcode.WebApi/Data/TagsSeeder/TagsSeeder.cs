using Streetcode.DAL.Entities.AdditionalContent;
using Streetcode.WebApi.InitialData.SeederExtensions;
using Streetcode.DAL.Persistence;

namespace Streetcode.WebApi.InitialData.TagsSeeder
{
    internal class TagsSeeder
    {
        public static async Task FillSeedAsync(
     StreetcodeDbContext context)
        {
            var entities = new List<Tag>
        {
            new()
            {
                Title = "writer"
            },
            new ()
            {
                Title = "artist"
            },
            new ()
            {
                Title = "composer"
            },
            new ()
            {
                Title = "victory"
            },
            new ()
            {
                Title = "Наукова школа"
            },
            new ()
            {
                Title = "Історія"
            },
            new ()
            {
                Title = "Політика"
            },
            new ()
            {
                Title = "Активіст",
            },
            new ()
            {
                Title = "Борці за незалежність",
            },
            new ()
            {
                Title = "Герої",
            }
        };

            await context.Tags.SeedIfEmptyAsync(
                entities,
                context);
        }
    }
}
