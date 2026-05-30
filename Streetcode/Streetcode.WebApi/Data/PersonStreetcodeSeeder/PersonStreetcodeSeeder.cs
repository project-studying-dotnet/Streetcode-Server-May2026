using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Streetcode.Types;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.PersonStreetcodeSeeder
{
    [ExcludeFromCodeCoverage]
    public static class PersonStreetcodeSeeder
    {
        public static async Task FillSeedAsync(StreetcodeDbContext context)
        {
            var now = DateTime.UtcNow;

            var entities = new List<PersonStreetcode>
            {
                new()
                {
                    Index = 1,
                    TransliterationUrl = "taras-shevchenko",
                    Teaser = "Тара́с Григо́рович Шевче́нко (25 лютого (9 березня) 1814, с. Моринці, Київська губернія," +
                        " Російська імперія (нині Звенигородський район, Черкаська область, Україна) — 26 лютого (10 березня) 1861, " +
                        "Санкт-Петербург, Російська імперія) — український поет, прозаїк, мислитель, живописець, гравер, етнограф, громадський діяч. " +
                        "Національний герой і символ України. Діяч українського національного руху, член Кирило-Мефодіївського братства. " +
                        "Академік Імператорської академії мистецтв",
                    ViewCount = 0,
                    CreatedAt = now,
                    DateString = "9 березня 1814 — 10 березня 1861",
                    EventStartOrPersonBirthDate = ToUtc(1814, 3, 9),
                    EventEndOrPersonDeathDate = ToUtc(1861, 3, 10),
                    FirstName = "Тарас",
                    Rank = "Григорович",
                    LastName = "Шевченко",
                    Title = "Тарас Шевченко",
                    Alias = "Кобзар",
                    AudioId = 1,
                    Status = StreetcodeStatus.Published
                },
                new()
                {
                    Index = 2,
                    TransliterationUrl = "roman-ratushnyi",
                    Teaser = "Роман був з тих, кому не байдуже. Небайдуже до свого Протасового Яру та своєї України. Талановитий, щедрий, запальний. З нового покоління українців, народжених за незалежності, мета яких — краща Україна. Інтелектуал, активіст, громадський діяч. Бунтар проти несправедливості: корупції, свавілля. Невтомний як у боротьбі з незаконною забудовою, так і в захисті рідної країни від ворога. Учасник Помаранчевої революції 2004 року та Революції гідності 2013–2014-го. Воїн, який заради України пожертвував власним життям.",
                    ViewCount = 1,
                    CreatedAt = now,
                    DateString = "5 липня 1997 – 9 червня 2022",
                    EventStartOrPersonBirthDate = ToUtc(1997, 7, 5),
                    EventEndOrPersonDeathDate = ToUtc(2022, 6, 9),
                    FirstName = "Роман",
                    LastName = "Ратушний",
                    Title = "Роман Ратушний (Сенека)",
                    Alias = "Сенека",
                    AudioId = 2,
                    Status = StreetcodeStatus.Published
                }
            };

            await context.Streetcodes.SeedIfEmptyAsync(entities, context);
        }

        private static DateTime ToUtc(int year, int month, int day)
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