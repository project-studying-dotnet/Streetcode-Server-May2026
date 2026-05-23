using System.Diagnostics.CodeAnalysis;
using Streetcode.DAL.Entities.Media;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.InitialData.SeederExtensions;

namespace Streetcode.WebApi.InitialData.VideosSeeder
{
    [ExcludeFromCodeCoverage]
    public static class VideosSeeder
    {
        private const string VideoUrl1 = "https://" + "www.youtube.com/" + "watch?v=VVFEi6lTpZk&ab_channel=%D0%9E%D1%81%D1%82%D0%B0%D0%BD%D0%BD%D1%96%D0%B9%D0%93%D0%B5%D1%82%D1%8C%D0%BC%D0%B0%D0%BD";
        private const string VideoUrl2 = "https://" + "www.youtube.com/" + "watch?v=YuoaECXH2Bc&ab_channel=%D0%A2%D0%B2%D0%BE%D1%8F%D0%9F%D1%96%D0%B4%D0%BF%D1%96%D0%BB%D1%8C%D0%BD%D0%B0%D0%93%D1%83%D0%BC%D0%B0%D0%BD%D1%96%D1%82%D0%B0%D1%80%D0%BA%D0%B0";

        public static async Task FillSeedAsync(StreetcodeDbContext context)
        {
            var entities = new List<Video>
            {
                new()
                {
                    Title = "audio1",
                    Description = "for streetcode1",
                    Url = VideoUrl1,
                    StreetcodeId = 1
                },
                new()
                {
                    Title = "Біографія Т.Г.Шевченка",
                    Url = VideoUrl2,
                    StreetcodeId = 2
                }
            };

            await context.Videos.SeedIfEmptyAsync(entities, context);
        }
    }
}