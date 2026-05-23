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
            var entities = new List<Art>
        {
            new()
            {
                ImageId = 19,
                Title = "Анатолій Федірко",
                Description = "Анатолій Федірко, «Український супрематичний політичний діяч Михайло Грушевський», 2019-2020 роки."
            },
            new()
            {
                ImageId = 20,
                Title = "Анатолій Федірко",
                Description = "Анатолій Федірко, «Український супрематичний політичний діяч Михайло Грушевський», 2019-2020 роки."
            },
            new()
            {
                ImageId = 21,
                Title = "Назар Дубів",
                Description = "Назар Дубів опублікував серію малюнків, у яких перетворив класиків української літератури та політичних діячів на сучасних модників"
            },
            new ()
            {
                ImageId = 22
            },
            new ()
            {
                ImageId = 22,
                Title = "Козаки на орбіті",
                Description = "«Козаки на орбіті» поєднує не тільки тему козаків, а й апелює до космічної тематики."
            },
            new ()
            {
                ImageId = 21,
                Title = "Січових стрільців",
                Description = "На вулиці Січових стрільців, 75 закінчили малювати мурал Михайла Грушевського на місці малюнка будинку з лелекою."
            },
            new ()
            {
                ImageId = 16,
                Title = "Січових стрільців",
                Description = "Some Description"
            },
            new ()
            {
                ImageId = 17,
                Title = "Січових стрільців",
                Description = "Some Description"
            },
            new ()
            {
                ImageId = 18,
                Title = "Січових стрільців",
                Description = "Some Description"
            },
            new ()
            {
                ImageId = 19,
                Title = "Січових стрільців",
                Description = "Some Description"
            }
        };

            await context.Arts.SeedIfEmptyAsync(
                entities,
                context);
        }
    }
}
