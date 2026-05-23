using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.News;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.NewsSeeder
{
    [ExcludeFromCodeCoverage]
    public static class NewsSeeder
    {
        public static async Task FillSeedAsync(
            StreetcodeDbContext context)
        {
            const string newsText1 = "<p>Встановлення таблички про Михайла Грушевського в м. Київ стало важливою подією для киян та гостей столиці..."; // ... (ваш повний текст)
            const string newsText2 = "<p>Привітаймо нового учасника команди - Терентьєва Даниїла!. Lorem ipsum dolor sit amet..."; // ... (ваш повний текст)
            const string newsText3 = "<p>Привітаймо нового учасника команди - Скам Мастера!. Lorem ipsum dolor sit amet..."; // ... (ваш повний текст)

            var data = new[]
            {
                new { T = "27 квітня встановлюємо перший стріткод!", TXT = newsText1, U = "first-streetcode", I = 24 },
                new { T = "Новий учасник команди!", TXT = newsText2, U = "danya", I = 28 },
                new { T = "Новий учасник команди!", TXT = newsText3, U = "scum", I = 29 }
            };

            var entities = data.Select(n => new News
            {
                Title = n.T,
                Text = n.TXT,
                URL = n.U,
                ImageId = n.I,
                CreationDate = DateTime.Now
            }).ToList();

            await context.News.SeedIfEmptyAsync(
            entities,
            context);
        }
    }
}