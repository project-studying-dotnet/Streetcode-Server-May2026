using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Streetcode.BLL.Interfaces.WebParsingUtils;

namespace Streetcode.BLL.Services.WebParsingUtils;

public class CsvAddressParserService : ICsvAddressParser
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CsvAddressParserService> _logger;
    private const string UkrPoshtaUrl = "https://www.ukrposhta.ua/files/shares/out/houses.zip";

    public CsvAddressParserService(HttpClient httpClient, ILogger<CsvAddressParserService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<TmpAddressModel>> ParseUkrPoshtaZipAsync(string tempDirectory)
    {
        string zipPath = Path.Combine(tempDirectory, "houses.zip");

        _logger.LogInformation("Downloading Ukrposhta archive...");
        var response = await _httpClient.GetAsync(UkrPoshtaUrl);
        response.EnsureSuccessStatusCode();
        await using (var fs = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await response.Content.CopyToAsync(fs);
        }

        _logger.LogInformation("Extracting archive...");
        ZipFile.ExtractToDirectory(zipPath, tempDirectory);

        string csvPath = Directory.GetFiles(tempDirectory, "*.csv").FirstOrDefault();
        if (string.IsNullOrEmpty(csvPath))
            throw new FileNotFoundException("CSV file not found in the archive.");

        return await ProcessCsvFileAsync(csvPath);
    }

    private async Task<List<TmpAddressModel>> ProcessCsvFileAsync(string csvPath)
    {
        var list = new List<TmpAddressModel>();
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var encoding = Encoding.GetEncoding("windows-1251");

        using var reader = new StreamReader(csvPath, encoding);
        await reader.ReadLineAsync();

        while (!reader.EndOfStream)
        {
            string line = await reader.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] values = line.Split(';');
            if (values.Length < 6) continue;

            list.Add(new TmpAddressModel
            {
                Oblast = values[0].Trim(),
                AdminRegionOld = values[1].Trim(),
                Gromada = values[2].Trim(),
                Community = values[3].Trim(),
                StreetName = OptimizeStreetname(values[4].Trim()),
                StreetType = values[5].Trim()
            });
        }

        return list.GroupBy(x => new { x.Oblast, x.Gromada, x.StreetName }).Select(g => g.First()).ToList();
    }

    private string OptimizeStreetname(string rawStreetName)
    {
        if (string.IsNullOrWhiteSpace(rawStreetName)) return rawStreetName;
        return rawStreetName.Replace("вул.", "вулиця").Replace("пр.", "проспект").Replace("пров.", "провулок").Replace("пл.", "площа");
    }
}