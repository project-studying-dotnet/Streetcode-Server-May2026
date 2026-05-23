using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Streetcode.BLL.Services.BlobStorageService;
using Streetcode.DAL.Persistence;

namespace Streetcode.WebApi.InitialData.SeedingHelper
{
    [ExcludeFromCodeCoverage]
    public static class SeedingHelper
    {
        public static async Task SeedFilesAsync<T>(
            StreetcodeDbContext dbContext,
            BlobService blobService,
            string jsonPath,
            string blobPath,
            Func<T, string> getBlobName,
            Func<T, string> getBase64,
            Action<IEnumerable<T>> addToDb)
                where T : class
        {
            if (!File.Exists(jsonPath))
            {
                return;
            }

            string jsonContent = await File.ReadAllTextAsync(jsonPath, System.Text.Encoding.UTF8);
            var items = JsonConvert.DeserializeObject<List<T>>(jsonContent);

            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                string blobName = getBlobName(item);
                string base64 = getBase64(item);

                if (string.IsNullOrEmpty(blobName) || string.IsNullOrEmpty(base64))
                {
                    continue;
                }

                string filePath = Path.Combine(blobPath, blobName);
                if (!File.Exists(filePath))
                {
                    var parts = blobName.Split('.');
                    blobService.SaveFileInStorageBase64(base64, parts[0], parts.Length > 1 ? parts[1] : "");
                }
            }

            addToDb(items);
            await dbContext.SaveChangesAsync();
        }
    }
}
