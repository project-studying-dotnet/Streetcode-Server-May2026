using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.Data.TeamMembersSeeder
{
    [ExcludeFromCodeCoverage]
    public class TeamMembersSeeder
    {
        [ExcludeFromCodeCoverage]
        public static async Task FillSeedAsync(
           StreetcodeDbContext context)
        {
            var entities = new List<TeamMember>
        {
            new()
            {
                FirstName = "Inna",
                LastName = "Krupnyk",
                ImageId = 25,
                Description = "У 1894 році Грушевський за рекомендацією Володимира Антоновича призначений\r\nна посаду ординарного професора",
                IsMain = true
            },
            new TeamMember
            {
                FirstName = "Danyil",
                LastName = "Terentiev",
                ImageId = 26,
                Description = "У 1894 році Грушевський за рекомендацією Володимира Антоновича призначений\r\nна посаду ординарного професора",
                IsMain = true
            },
            new TeamMember
            {
                FirstName = "Nadia",
                LastName = "Kischchuk",
                ImageId = 27,
                Description = "У 1894 році Грушевський за рекомендацією Володимира Антоновича призначений\r\nна посаду ординарного професора",
                IsMain = true
            }
        };

            await context.TeamMembers.SeedIfEmptyAsync(
                entities,
                context);
        }
    }
}
