using Streetcode.DAL.Entities.Media.Images;
using Streetcode.WebApi.InitialData.SeederExtensions;
using Streetcode.DAL.Persistence;

namespace Streetcode.WebApi.InitialData.ArtsSeeder
{
    internal class ArtsSeeder
    {
        public static async Task FillSeedAsync(
     StreetcodeDbContext context)
        {
        var data = new[]
            {
                new { Id = 19, T = "Анатолій Федірко", D = "Анатолій Федірко, «Український супрематичний політичний діяч Михайло Грушевський», 2019-2020 роки." },
                new { Id = 20, T = "Анатолій Федірко", D = "Анатолій Федірко, «Український супрематичний політичний діяч Михайло Грушевський», 2019-2020 роки." },
                new { Id = 21, T = "Назар Дубів", D = "Назар Дубів опублікував серію малюнків, у яких перетворив класиків української літератури та політичних діячів на сучасних модників" },
                new { Id = 22, T = "Козаки на орбіті", D = "«Козаки на орбіті» поєднує не тільки тему козаків, а й апелює до космічної тематики." },
                new { Id = 21, T = "Січових стрільців", D = "На вулиці Січових стрільців, 75 закінчили малювати мурал Михайла Грушевського на місці малюнка будинку з лелекою." },
                new { Id = 16, T = "Січових стрільців", D = "Some Description" },
                new { Id = 17, T = "Січових стрільців", D = "Some Description" },
                new { Id = 18, T = "Січових стрільців", D = "Some Description" },
                new { Id = 19, T = "Січових стрільців", D = "Some Description" }
            };

        var entities = data.Select(item => new Art
        {
            ImageId = item.Id,
            Title = item.T,
            Description = item.D
        }).ToList();

        await context.Arts.SeedIfEmptyAsync(
            entities,
            context);
        }
    }
}
