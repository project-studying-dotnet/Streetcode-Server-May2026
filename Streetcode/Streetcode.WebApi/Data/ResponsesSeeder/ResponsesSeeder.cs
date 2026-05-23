using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Feedback;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.ResponsesSeeder
{
    [ExcludeFromCodeCoverage]
    public static class ResponsesSeeder
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
