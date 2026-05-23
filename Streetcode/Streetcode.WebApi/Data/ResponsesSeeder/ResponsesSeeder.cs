using Streetcode.DAL.Entities.Feedback;
using Streetcode.WebApi.InitialData.SeederExtensions;
using Streetcode.DAL.Persistence;

namespace Streetcode.WebApi.InitialData.ResponsesSeeder
{
    public class ResponsesSeeder
    {
        public static async Task FillSeedAsync(
     StreetcodeDbContext context)
        {
            var entities = new List<Response>
        {
            new()
            {
                Name = "Alex",
                Description = "Good Job",
                Email = "dmytrobuchkovsky@gmail.com"
            },
            new ()
            {
                Name = "Danyil",
                Description = "Nice project",
                Email = "dt210204@gmail.com"
            }
        };

            await context.Responses.SeedIfEmptyAsync(
                entities,
                context);
        }
    }
}
