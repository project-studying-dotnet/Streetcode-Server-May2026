using Streetcode.DAL.Entities.Team;
using Streetcode.DAL.Enums;
using Streetcode.WebApi.InitialData.SeederExtensions;
using Streetcode.DAL.Persistence;

namespace Streetcode.WebApi.InitialData.TeamMemberLinksSeeder
{
    public class TeamMemberLinksSeeder
    {
        public static async Task FillSeedAsync(
       StreetcodeDbContext context)
        {
        var entities = new List<TeamMemberLink>
        {
            new()
            {
                LogoType = LogoType.YouTube,
                TargetUrl = "https://www.youtube.com/watch?v=8kCnOqvmEp0&ab_channel=JL%7C%D0%AE%D0%9B%D0%86%D0%AF%D0%9B%D0%A3%D0%A9%D0%98%D0%9D%D0%A1%D0%AC%D0%9A%D0%90",
                TeamMemberId = 1,
            },
            new ()
            {
                LogoType = LogoType.Facebook,
                TargetUrl = "https://www.youtube.com/watch?v=8kCnOqvmEp0&ab_channel=JL%7C%D0%AE%D0%9B%D0%86%D0%AF%D0%9B%D0%A3%D0%A9%D0%98%D0%9D%D0%A1%D0%AC%D0%9A%D0%90",
                TeamMemberId = 1,
            },
            new ()
            {
                LogoType = LogoType.Instagram,
                TargetUrl = "https://www.youtube.com/watch?v=8kCnOqvmEp0&ab_channel=JL%7C%D0%AE%D0%9B%D0%86%D0%AF%D0%9B%D0%A3%D0%A9%D0%98%D0%9D%D0%A1%D0%AC%D0%9A%D0%90",
                TeamMemberId = 1,
            },
            new ()
            {
                LogoType = LogoType.Twitter,
                TargetUrl = "https://www.youtube.com/watch?v=8kCnOqvmEp0&ab_channel=JL%7C%D0%AE%D0%9B%D0%86%D0%AF%D0%9B%D0%A3%D0%A9%D0%98%D0%9D%D0%A1%D0%AC%D0%9A%D0%90",
                TeamMemberId = 1,
            },
            new ()
            {
                LogoType = LogoType.YouTube,
                TargetUrl = "https://www.youtube.com/watch?v=8kCnOqvmEp0&ab_channel=JL%7C%D0%AE%D0%9B%D0%86%D0%AF%D0%9B%D0%A3%D0%A9%D0%98%D0%9D%D0%A1%D0%AC%D0%9A%D0%90",
                TeamMemberId = 2,
            },
            new ()
            {
                LogoType = LogoType.Facebook,
                TargetUrl = "https://www.youtube.com/watch?v=8kCnOqvmEp0&ab_channel=JL%7C%D0%AE%D0%9B%D0%86%D0%AF%D0%9B%D0%A3%D0%A9%D0%98%D0%9D%D0%A1%D0%AC%D0%9A%D0%90",
                TeamMemberId = 2,
            },
            new ()
            {
                LogoType = LogoType.Instagram,
                TargetUrl = "https://www.youtube.com/watch?v=8kCnOqvmEp0&ab_channel=JL%7C%D0%AE%D0%9B%D0%86%D0%AF%D0%9B%D0%A3%D0%A9%D0%98%D0%9D%D0%A1%D0%AC%D0%9A%D0%90",
                TeamMemberId = 2,
            },
            new ()
            {
                LogoType = LogoType.Twitter,
                TargetUrl = "https://www.youtube.com/watch?v=8kCnOqvmEp0&ab_channel=JL%7C%D0%AE%D0%9B%D0%86%D0%AF%D0%9B%D0%A3%D0%A9%D0%98%D0%9D%D0%A1%D0%AC%D0%9A%D0%90",
                TeamMemberId = 2,
            },
            new ()
            {
                LogoType = LogoType.YouTube,
                TargetUrl = "https://www.youtube.com/watch?v=8kCnOqvmEp0&ab_channel=JL%7C%D0%AE%D0%9B%D0%86%D0%AF%D0%9B%D0%A3%D0%A9%D0%98%D0%9D%D0%A1%D0%AC%D0%9A%D0%90",
                TeamMemberId = 3,
            },
            new ()
            {
                LogoType = LogoType.Facebook,
                TargetUrl = "https://www.youtube.com/watch?v=8kCnOqvmEp0&ab_channel=JL%7C%D0%AE%D0%9B%D0%86%D0%AF%D0%9B%D0%A3%D0%A9%D0%98%D0%9D%D0%A1%D0%AC%D0%9A%D0%90",
                TeamMemberId = 3,
            },
            new ()
            {
                LogoType = LogoType.Instagram,
                TargetUrl = "https://www.youtube.com/watch?v=8kCnOqvmEp0&ab_channel=JL%7C%D0%AE%D0%9B%D0%86%D0%AF%D0%9B%D0%A3%D0%A9%D0%98%D0%9D%D0%A1%D0%AC%D0%9A%D0%90",
                TeamMemberId = 3,
            },
            new ()
            {
                LogoType = LogoType.Twitter,
                TargetUrl = "https://www.youtube.com/watch?v=8kCnOqvmEp0&ab_channel=JL%7C%D0%AE%D0%9B%D0%86%D0%AF%D0%9B%D0%A3%D0%A9%D0%98%D0%9D%D0%A1%D0%AC%D0%9A%D0%90",
                TeamMemberId = 3,
            }
        };

        await context.TeamMemberLinks.SeedIfEmptyAsync(
            entities,
            context);
        }
    }
}
