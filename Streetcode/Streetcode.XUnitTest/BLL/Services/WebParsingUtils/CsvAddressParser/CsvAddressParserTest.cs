using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Streetcode.BLL.Services.WebParsingUtils;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Services.WebParsingUtils;

public class CsvAddressParserServiceTests : IDisposable
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly Mock<ILogger<CsvAddressParserService>> _loggerMock;
    private readonly string _testTempDirectory;

    public CsvAddressParserServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _loggerMock = new Mock<ILogger<CsvAddressParserService>>();

        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        _testTempDirectory = Path.Combine(Path.GetTempPath(), "Streetcode_Tests_" + Guid.NewGuid());
        Directory.CreateDirectory(_testTempDirectory);
    }

    private static byte[] CreateMockZipArchive(string csvContent)
    {
        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            var demoFile = archive.CreateEntry("addresses.csv");
            using var entryStream = demoFile.Open();
            using var writer = new StreamWriter(entryStream, Encoding.GetEncoding("windows-1251"));
            writer.Write(csvContent);
        }
        return memoryStream.ToArray();
    }

    public void Dispose()
    {
        if (Directory.Exists(_testTempDirectory))
        {
            Directory.Delete(_testTempDirectory, true);
        }
        GC.SuppressFinalize(this);
    }

    private CsvAddressParserService CreateService(string downloadUrl = "https://fake-ukrposhta.com/houses.zip")
    {
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        var settings = new UkrPoshtaParserSettings { DownloadUrl = downloadUrl };
        var options = Options.Create(settings);

        return new CsvAddressParserService(httpClient, _loggerMock.Object, options);
    }

    [Fact]
    public async Task ParseUkrPoshtaZipAsync_WithValidZipAndCsv_ReturnsCorrectlyParsedAndGroupedAddresses()
    {
        var service = CreateService();

        var csvContent = "Oblast;AdminOld;Gromada;Community;Street;Type\n" +
                         "Львівська;Львівський;Львівська;Львів;вул. Степана Бандери;вулиця\n" +
                         "Львівська;Львівський;Львівська;Львів;вул. Степана Бандери;вулиця\n" +
                         "Київська;Броварський;Броварська;Бровари;пр. Незалежності;проспект";

        var zipBytes = CreateMockZipArchive(csvContent);

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new ByteArrayContent(zipBytes)
            });

        var result = await service.ParseUkrPoshtaZipAsync(_testTempDirectory);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        var first = result[0];
        Assert.Equal("Львівська", first.Oblast);
        Assert.Equal("Львівська", first.Gromada);
        Assert.Equal("вулиця Степана Бандери", first.StreetName);

        var second = result[1];
        Assert.Equal("Київська", second.Oblast);
        Assert.Equal("проспект Незалежності", second.StreetName);
    }

    [Fact]
    public async Task ParseUkrPoshtaZipAsync_WhenUrlIsNotConfigured_ThrowsInvalidOperationException()
    {
        var service = CreateService(downloadUrl: "");

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.ParseUkrPoshtaZipAsync(_testTempDirectory));
    }

    [Fact]
    public async Task ParseUkrPoshtaZipAsync_WhenHttpErrorOccurs_ThrowsHttpRequestException()
    {
        var service = CreateService();

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });

        await Assert.ThrowsAsync<HttpRequestException>(() =>
            service.ParseUkrPoshtaZipAsync(_testTempDirectory));
    }

    [Fact]
    public async Task ParseUkrPoshtaZipAsync_WhenZipDoesNotContainCsv_ThrowsFileNotFoundException()
    {
        var service = CreateService();

        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
        }
        var emptyZipBytes = memoryStream.ToArray();

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new ByteArrayContent(emptyZipBytes)
            });

        await Assert.ThrowsAsync<FileNotFoundException>(() =>
            service.ParseUkrPoshtaZipAsync(_testTempDirectory));
    }
}