using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Comments;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.Data.CommentsSeeder
{
    [ExcludeFromCodeCoverage]
    internal static class CommentsSeeder
    {
        public static async Task FillSeedAsync(StreetcodeDbContext context)
        {
            var comments = new List<Comment>
            {
                // StreetcodeId = 1 (Шевченко) — top-level
                new()
                {
                    Id = 1,
                    Text = "Неймовірна особистість, яка залишила незгладимий слід в українській культурі. Дякую за цей стріткод!",
                    UserId = 3,
                    StreetcodeId = 1,
                    CreatedAt = new DateTime(2024, 3, 9, 10, 0, 0, DateTimeKind.Utc),
                },
                new()
                {
                    Id = 2,
                    Text = "Цікаво було дізнатись про викуп з кріпацтва. Вражаюча історія боротьби за свободу.",
                    UserId = 3,
                    StreetcodeId = 1,
                    CreatedAt = new DateTime(2024, 3, 9, 11, 30, 0, DateTimeKind.Utc),
                },
                new()
                {
                    Id = 3,
                    Text = "Творчість Шевченка — це не лише поезія, а й заклик до національної свідомості кожного українця.",
                    UserId = 3,
                    StreetcodeId = 1,
                    CreatedAt = new DateTime(2024, 3, 10, 9, 0, 0, DateTimeKind.Utc),
                },

                // Reply to comment 1
                new()
                {
                    Id = 4,
                    Text = "Повністю згоден! Особливо вражає факт про лотерею з портретом Жуковського.",
                    UserId = 3,
                    StreetcodeId = 1,
                    ParentCommentId = 1,
                    CreatedAt = new DateTime(2024, 3, 9, 12, 0, 0, DateTimeKind.Utc),
                },
                new()
                {
                    Id = 5,
                    Text = "Так, і той факт, що 2500 рублів на ті часи — величезна сума, лише підкреслює, як цінували його талант.",
                    UserId = 3,
                    StreetcodeId = 1,
                    ParentCommentId = 1,
                    CreatedAt = new DateTime(2024, 3, 9, 14, 0, 0, DateTimeKind.Utc),
                },

                // Nested reply to comment 4
                new()
                {
                    Id = 6,
                    Text = "Дякую за уточнення! Це справді надихає.",
                    UserId = 3,
                    StreetcodeId = 1,
                    ParentCommentId = 4,
                    CreatedAt = new DateTime(2024, 3, 9, 15, 0, 0, DateTimeKind.Utc),
                },

                // StreetcodeId = 2 (Ратушний) — top-level
                new()
                {
                    Id = 7,
                    Text = "Роман — справжній герой нашого часу. Вічна пам'ять борцю за вільну Україну.",
                    UserId = 3,
                    StreetcodeId = 2,
                    CreatedAt = new DateTime(2024, 4, 1, 8, 0, 0, DateTimeKind.Utc),
                },
                new()
                {
                    Id = 8,
                    Text = "Вражає, як молода людина мала таку силу духу і відданість громаді. Надихає на дію.",
                    UserId = 3,
                    StreetcodeId = 2,
                    CreatedAt = new DateTime(2024, 4, 1, 10, 0, 0, DateTimeKind.Utc),
                },
                new()
                {
                    Id = 9,
                    Text = "Його слова «загинув далеко від Тебе, Києве, але загинув за Тебе» — це заповіт для всіх нас.",
                    UserId = 3,
                    StreetcodeId = 2,
                    CreatedAt = new DateTime(2024, 4, 2, 9, 0, 0, DateTimeKind.Utc),
                },

                // Reply to comment 7
                new()
                {
                    Id = 10,
                    Text = "Пам'ятаємо і пишаємося. Такі люди і є обличчям нової України.",
                    UserId = 3,
                    StreetcodeId = 2,
                    ParentCommentId = 7,
                    CreatedAt = new DateTime(2024, 4, 1, 11, 0, 0, DateTimeKind.Utc),
                },
            };

            await context.Comments.SeedIfEmptyAsync(comments, context);
        }
    }
}
