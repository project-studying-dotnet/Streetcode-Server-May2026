using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.TeamMemberLinksSeeder
{
    [ExcludeFromCodeCoverage]
    public class TeamMemberLinksSeeder
    {
        public static async Task FillSeedAsync(
       StreetcodeDbContext context)
        {
            var url = "https://www.youtube.com/watch?v=8kCnOqvmEp0&ab_channel=JL%7C%D0%AE%D0%9B%D0%86%D0%AF%D0%9B%D0%A3%D0%A9%D0%98%D0%9D%D0%A1%D0%AC%D0%9A%D0%90";

            var data = new[]
            {
                (LogoType.YouTube, 1), (LogoType.Facebook, 1), (LogoType.Instagram, 1), (LogoType.Twitter, 1),
                (LogoType.YouTube, 2), (LogoType.Facebook, 2), (LogoType.Instagram, 2), (LogoType.Twitter, 2),
                (LogoType.YouTube, 3), (LogoType.Facebook, 3), (LogoType.Instagram, 3), (LogoType.Twitter, 3)
            };

            var entities = data.Select(item => new TeamMemberLink
            {
                LogoType = item.Item1,
                TargetUrl = url,
                TeamMemberId = item.Item2
            }).ToList();

            await context.TeamMemberLinks.SeedIfEmptyAsync(
            entities,
            context);
        }
    }
}
