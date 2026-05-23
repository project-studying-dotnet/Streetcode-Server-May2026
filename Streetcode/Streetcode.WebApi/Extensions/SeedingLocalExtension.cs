using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Streetcode.BLL.Services.BlobStorageService;
using Streetcode.DAL.Entities.Media;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.WebApi.Data.TeamMembersSeeder;
using Streetcode.WebApi.InitialData.ArtsSeeder;
using Streetcode.WebApi.InitialData.FactsSeeder;
using Streetcode.WebApi.InitialData.HistoricalContextsSeeder;
using Streetcode.WebApi.InitialData.HistoricalContextsTimelinesSeeder;
using Streetcode.WebApi.InitialData.ImageDetailsesSeeder;
using Streetcode.WebApi.InitialData.NewsSeeder;
using Streetcode.WebApi.InitialData.PartnerSourceLinksSeeder;
using Streetcode.WebApi.InitialData.PartnersSeeder;
using Streetcode.WebApi.InitialData.PersonStreetcodeSeeder;
using Streetcode.WebApi.InitialData.PositionsSeeder;
using Streetcode.WebApi.InitialData.RelatedFiguresSeeder;
using Streetcode.WebApi.InitialData.RelatedTerms;
using Streetcode.WebApi.InitialData.ResponsesSeeder;
using Streetcode.WebApi.InitialData.SeedingHelper;
using Streetcode.WebApi.InitialData.SourceLinkCategorySeeder;
using Streetcode.WebApi.InitialData.StreetcodeArtsSeeder;
using Streetcode.WebApi.InitialData.StreetcodeCategoryContentSeeder;
using Streetcode.WebApi.InitialData.StreetcodeCoordinatesSeeder;
using Streetcode.WebApi.InitialData.StreetcodeImagesSeeder;
using Streetcode.WebApi.InitialData.StreetcodePartnersSeeder;
using Streetcode.WebApi.InitialData.StreetcodeTagIndexSeeder;
using Streetcode.WebApi.InitialData.SubtitlesSeeder;
using Streetcode.WebApi.InitialData.TagsSeeder;
using Streetcode.WebApi.InitialData.TeamMemberLinksSeeder;
using Streetcode.WebApi.InitialData.TeamMemberPositionSeeder;
using Streetcode.WebApi.InitialData.TermsSeeder;
using Streetcode.WebApi.InitialData.TextsSeeder;
using Streetcode.WebApi.InitialData.TimelineItemsSeeder;
using Streetcode.WebApi.InitialData.TransactionLinkSeeder;
using Streetcode.WebApi.InitialData.UserSeeder;
using Streetcode.WebApi.InitialData.VideosSeeder;

namespace Streetcode.WebApi.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class SeedingLocalExtension
    {
        public static async Task SeedDataAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                string blobPath = app.Configuration.GetValue<string>("Blob:BlobStorePath")
                    ?? throw new InvalidOperationException("Critical error: Path not specified 'Blob:BlobStorePath' в конфигурации.");

                Directory.CreateDirectory(blobPath);
                var dbContext = scope.ServiceProvider.GetRequiredService<StreetcodeDbContext>();
                var blobOptions = app.Services.GetRequiredService<IOptions<BlobEnvironmentVariables>>();
                var repo = new RepositoryWrapper(dbContext);
                var blobService = new BlobService(blobOptions, repo);
                string initialDataImagePath = "../Streetcode.DAL/InitialData/images.json";
                string initialDataAudioPath = "../Streetcode.DAL/InitialData/audios.json";

                await UserSeeder.FillSeedAsync(dbContext);
                await PositionsSeeder.FillSeedAsync(dbContext);

                if (!await dbContext.Images.AnyAsync())
                {
                    await SeedingHelper.SeedFilesAsync<Image>(dbContext, blobService, initialDataImagePath, blobPath, i => i.BlobName ?? string.Empty, i => i.Base64 ?? string.Empty, list => dbContext.Images.AddRange(list));

                    if (!await dbContext.Audios.AnyAsync())
                    {
                        await SeedingHelper.SeedFilesAsync<Audio>(dbContext, blobService, initialDataAudioPath, blobPath, a => a.BlobName ?? string.Empty, a => a.Base64 ?? string.Empty, list => dbContext.Audios.AddRange(list));
                    }

                    await ResponsesSeeder.FillSeedAsync(dbContext);

                    await NewsSeeder.FillSeedAsync(dbContext);

                    if (!await dbContext.Terms.AnyAsync())
                    {
                        await TermsSeeder.FillSeedAsync(dbContext);

                        await RelatedTerms.FillSeedAsync(dbContext);
                    }

                    if (!await dbContext.TeamMembers.AnyAsync())
                    {
                        await TeamMembersSeeder.FillSeedAsync(dbContext);

                        if (!await dbContext.Positions.AnyAsync())
                        {
                            await PositionsSeeder.FillSeedAsync(dbContext);

                            await TeamMemberPositionSeeder.FillSeedAsync(dbContext);

                            await TeamMemberLinksSeeder.FillSeedAsync(dbContext);
                        }

                        if (!await dbContext.Audios.AnyAsync() && !await dbContext.Streetcodes.AnyAsync())
                        {
                            await PersonStreetcodeSeeder.FillSeedAsync(dbContext);

                            await SubtitlesSeeder.FillSeedAsync(dbContext);

                            await StreetcodeCoordinatesSeeder.FillSeedAsync(dbContext);

                            await VideosSeeder.FillSeedAsync(dbContext);

                            if (!await dbContext.Partners.AnyAsync())
                            {
                                await PartnersSeeder.FillSeedAsync(dbContext);

                                await PartnerSourceLinksSeeder.FillSeedAsync(dbContext);

                                await StreetcodePartnersSeeder.FillSeedAsync(dbContext);
                            }

                            if (!await dbContext.Arts.AnyAsync())
                            {
                                await ArtsSeeder.FillSeedAsync(dbContext);

                                await StreetcodeArtsSeeder.FillSeedAsync(dbContext);
                            }

                            await TextsSeeder.FillSeedAsync(dbContext);

                            if (!await dbContext.TimelineItems.AnyAsync())
                            {
                                await TimelineItemsSeeder.FillSeedAsync(dbContext);

                                if (!await dbContext.HistoricalContexts.AnyAsync())
                                {
                                    await HistoricalContextsSeeder.FillSeedAsync(dbContext);

                                    await HistoricalContextsTimelinesSeeder.FillSeedAsync(dbContext);
                                }
                            }

                            await TransactionLinkSeeder.FillSeedAsync(dbContext);

                            if (!await dbContext.Facts.AnyAsync())
                            {
                                await FactsSeeder.FillSeedAsync(dbContext);
                                await ImageDetailsesSeeder.FillSeedAsync(dbContext);
                            }

                            if (!await dbContext.SourceLinks.AnyAsync())
                            {
                                await SourceLinkCategorySeeder.FillSeedAsync(dbContext);

                                await StreetcodeCategoryContentSeeder.FillSeedAsync(dbContext);
                            }

                            await RelatedFiguresSeeder.FillSeedAsync(dbContext);

                            await StreetcodeImagesSeeder.FillSeedAsync(dbContext);

                            if (!await dbContext.Tags.AnyAsync())
                            {
                                await TagsSeeder.FillSeedAsync(dbContext);

                                await StreetcodeTagIndexSeeder.FillSeedAsync(dbContext);
                            }
                        }

                        await dbContext.SaveChangesAsync();
                    }
                }
            }
        }
    }
}
