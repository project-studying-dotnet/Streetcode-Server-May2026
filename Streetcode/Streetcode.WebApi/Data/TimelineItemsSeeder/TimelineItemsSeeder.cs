using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.TimelineItemsSeeder
{
    [ExcludeFromCodeCoverage]
    public static class TimelineItemsSeeder
    {
        private const string PetersburgTitle = "Перші роки в Петербурзі";
        private const string PetersburgDesc = "Переїхавши 1831 року з Вільна до Петербурга, поміщик П. Енгельгардт узяв із собою Шевченка, " +
            "а щоб згодом мати зиск на художніх творах власного «покоєвого художника», підписав контракт й віддав його" +
            " в науку на чотири роки до живописця В. Ширяєва, у якого й замешкав Тарас до 1838 року.";

        public static async Task FillSeedAsync(StreetcodeDbContext context)
        {
            var data = new[]
            {
                new { D = ToUtc(1831), T = PetersburgTitle, Desc = PetersburgDesc, SId = 1, P = DateViewPattern.Year },
                new { D = ToUtc(1830), T = "Учень Петербурзької академії мистецтв", Desc = "Засвідчивши свою відпускну...", SId = 1, P = DateViewPattern.Year },
                new { D = ToUtc(1832), T = PetersburgTitle, Desc = PetersburgDesc, SId = 1, P = DateViewPattern.Year },
                new { D = ToUtc(1833), T = PetersburgTitle, Desc = PetersburgDesc, SId = 1, P = DateViewPattern.Year },
                new { D = ToUtc(1834), T = PetersburgTitle, Desc = PetersburgDesc, SId = 1, P = DateViewPattern.Year },
                new { D = ToUtc(1835), T = PetersburgTitle, Desc = PetersburgDesc, SId = 1, P = DateViewPattern.Year },
                new { D = ToUtc(1836), T = PetersburgTitle, Desc = PetersburgDesc, SId = 1, P = DateViewPattern.Year },
                new { D = ToUtc(1997, 7, 5), T = "Народився", Desc = "...", SId = 2, P = DateViewPattern.DateMonthYear },
                new { D = ToUtc(2012), T = "Обирає фах", Desc = "...", SId = 2, P = DateViewPattern.Year },
                new { D = ToUtc(2013, 11, 30), T = "Проти несправедливості", Desc = "...", SId = 2, P = DateViewPattern.DateMonthYear },
                new { D = ToUtc(2013, 12, 30), T = "«Знаю, що роблю»", Desc = "...", SId = 2, P = DateViewPattern.MonthYear },
                new { D = ToUtc(2014), T = "Боротьба лише починається", Desc = "...", SId = 2, P = DateViewPattern.Year },
                new { D = ToUtc(2018), T = "Захистимо Протасів Яр", Desc = "...", SId = 2, P = DateViewPattern.Year },
                new { D = ToUtc(2019), T = "Погрози", Desc = "...", SId = 2, P = DateViewPattern.Year },
                new { D = ToUtc(2020, 6, 1), T = "Перемога в суді", Desc = "...", SId = 2, P = DateViewPattern.SeasonYear },
                new { D = ToUtc(2020, 12, 30), T = "Досвід політика", Desc = "...", SId = 2, P = DateViewPattern.SeasonYear },
                new { D = ToUtc(2021), T = "Домашній арешт", Desc = "...", SId = 2, P = DateViewPattern.Year },
                new { D = ToUtc(2021), T = "Сфабрикована справа", Desc = "...", SId = 2, P = DateViewPattern.Year },
                new { D = ToUtc(2022, 2, 24), T = "Підрозділ Протасового", Desc = "...", SId = 2, P = DateViewPattern.DateMonthYear },
                new { D = ToUtc(2022, 3, 1), T = "Холодний Яр", Desc = "...", SId = 2, P = DateViewPattern.SeasonYear },
                new { D = ToUtc(2022, 6, 9), T = "Завжди 24", Desc = "...", SId = 2, P = DateViewPattern.DateMonthYear },
                new { D = ToUtc(2022, 6, 18), T = "Байкове. Вічність", Desc = "...", SId = 2, P = DateViewPattern.DateMonthYear },
                new { D = ToUtc(2022, 9, 8), T = "Вулиця Ратушного", Desc = "...", SId = 2, P = DateViewPattern.DateMonthYear },
                new { D = ToUtc(2022, 9, 13), T = "За мужність", Desc = "...", SId = 2, P = DateViewPattern.DateMonthYear }
            };

            var entities = data.Select(item => new TimelineItem
            {
                Date = item.D,
                Title = item.T,
                Description = item.Desc,
                StreetcodeId = item.SId,
                DateViewPattern = item.P
            }).ToList();

            await context.TimelineItems.SeedIfEmptyAsync(entities, context);
        }

        private static DateTime ToUtc(
            int year,
            int month = 1,
            int day = 1)
        {
            return new DateTime(
                year,
                month,
                day,
                0,
                0,
                0,
                DateTimeKind.Utc);
        }
    }
}