using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.TeamMemberLinksSeeder
{
    [ExcludeFromCodeCoverage]
    public static class TeamMemberLinksSeeder
    {
        public static async Task FillSeedAsync(
       StreetcodeDbContext context)
        {
            var url = "https://www.youtube.com/" + "watch?v=8kCnOqvmEp0&ab_channel=JL%7C%D0%AE%D0%9B%D0%86%D0%AF%D0%9B%D0%A3%D0%A9%D0%98%D0%9D%D0%A1%D0%AC%D0%9A%D0%90";

            var data = new[]
            {
                new { Type = LogoType.YouTube, MemberId = 1 }, new { Type = LogoType.Facebook, MemberId = 1 }, new { Type = LogoType.Instagram, MemberId = 1 }, new { Type = LogoType.Twitter, MemberId = 1 },
                new { Type = LogoType.YouTube, MemberId = 2 }, new { Type = LogoType.Facebook, MemberId = 2 }, new { Type = LogoType.Instagram, MemberId = 2 }, new { Type = LogoType.Twitter, MemberId = 2 },
                new { Type = LogoType.YouTube, MemberId = 3 }, new { Type = LogoType.Facebook, MemberId = 3 }, new { Type = LogoType.Instagram, MemberId = 3 }, new { Type = LogoType.Twitter, MemberId = 3 }
            };

            var entities = data.Select(item => new TeamMemberLink
            {
                LogoType = item.Type,
                TargetUrl = url,
                TeamMemberId = item.MemberId
            }).ToList();

            await context.TeamMemberLinks.SeedIfEmptyAsync(
            entities,
            context);
        }
    }
}
