using Streetcode.DAL.Entities.AdditionalContent;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.SubtitlesSeeder
{
    public class SubtitlesSeeder
    {
        public static async Task FillSeedAsync(
        StreetcodeDbContext context)
        {
        var entities = new List<Subtitle>
        {
            new()
            {
                SubtitleText = "Developers: StreedCodeTeam, made with love and passion, some more text, and more text. There was Danya",
                StreetcodeId = 1
            },
            new Subtitle
            {
                SubtitleText = "Developers: StreedCodeTeam, made with love and passion, some more text, and more text. There was Danya",
                StreetcodeId = 2
            }
        };

        await context.Subtitles.SeedIfEmptyAsync(
            entities,
            context);
        }
    }
}
