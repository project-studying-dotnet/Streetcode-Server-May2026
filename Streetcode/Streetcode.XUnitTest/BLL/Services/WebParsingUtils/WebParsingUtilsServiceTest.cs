using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Streetcode.BLL.Interfaces.WebParsingUtils;
using Streetcode.BLL.Services.WebParsingUtils;
using Streetcode.DAL.Entities.Toponyms;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Services.WebParsingUtils;

public class WebParsingUtilsServiceTests
{
    private readonly Mock<ICsvAddressParser> _csvParserMock;
    private readonly Mock<IGeocoding> _geocodingMock;
    private readonly Mock<IToponymData> _toponymDataMock;
    private readonly Mock<ILogger<WebParsingUtilsService>> _loggerMock;
    private readonly WebParsingUtilsService _service;

    public WebParsingUtilsServiceTests()
    {
        _csvParserMock = new Mock<ICsvAddressParser>();
        _geocodingMock = new Mock<IGeocoding>();
        _toponymDataMock = new Mock<IToponymData>();
        _loggerMock = new Mock<ILogger<WebParsingUtilsService>>();

        _service = new WebParsingUtilsService(
            _csvParserMock.Object,
            _geocodingMock.Object,
            _toponymDataMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task ParseZipFileFromWebAsync_WithValidAddressesAndCoordinates_SuccessfullySavesToDb()
    {
        var mockAddresses = new List<TmpAddressModel>
        {
            new TmpAddressModel
            {
                Oblast = "Львівська",
                AdminRegionOld = "Львівський",
                Gromada = "Львівська",
                Community = "Львів",
                StreetName = "вулиця Степана Бандери",
                StreetType = "вулиця"
            }
        };

        _csvParserMock
            .Setup(x => x.ParseUkrPoshtaZipAsync(It.IsAny<string>()))
            .ReturnsAsync(mockAddresses);

        _geocodingMock
            .Setup(x => x.GetCoordinatesAsync("Львівська область, вулиця Степана Бандери"))
            .ReturnsAsync((49.8397m, 24.0297m));

        await _service.ParseZipFileFromWebAsync();

        _csvParserMock.Verify(x => x.ParseUkrPoshtaZipAsync(It.IsAny<string>()), Times.Once);
        _geocodingMock.Verify(x => x.GetCoordinatesAsync(It.IsAny<string>()), Times.Once);
        _toponymDataMock.Verify(x => x.RefreshToponymsInDbAsync(It.Is<List<Toponym>>(list =>
            list.Count == 1 &&
            list[0].StreetName == "вулиця Степана Бандери" &&
            list[0].Coordinate.Latitude == 49.8397m
        )), Times.Once);
    }

    [Fact]
    public async Task ParseZipFileFromWebAsync_WhenGeocodingReturnsNull_DoesNotSaveToDb()
    {
        var mockAddresses = new List<TmpAddressModel>
        {
            new TmpAddressModel { Oblast = "Київська", StreetName = "Невідома Вулиця" }
        };

        _csvParserMock
            .Setup(x => x.ParseUkrPoshtaZipAsync(It.IsAny<string>()))
            .ReturnsAsync(mockAddresses);

        _geocodingMock
            .Setup(x => x.GetCoordinatesAsync(It.IsAny<string>()))
            .ReturnsAsync(((decimal Lat, decimal Lon)?)null);

        await _service.ParseZipFileFromWebAsync();

        _toponymDataMock.Verify(x => x.RefreshToponymsInDbAsync(It.IsAny<List<Toponym>>()), Times.Never);
    }

    [Fact]
    public async Task ParseZipFileFromWebAsync_WhenNoAddressesParsed_DoesNotCallGeocodingOrDb()
    {
        _csvParserMock
            .Setup(x => x.ParseUkrPoshtaZipAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<TmpAddressModel>());

        await _service.ParseZipFileFromWebAsync();

        _geocodingMock.Verify(x => x.GetCoordinatesAsync(It.IsAny<string>()), Times.Never);
        _toponymDataMock.Verify(x => x.RefreshToponymsInDbAsync(It.IsAny<List<Toponym>>()), Times.Never);
    }

    [Fact]
    public async Task ParseZipFileFromWebAsync_WhenExceptionOccursInParser_LogsErrorAndCleansUpDirectory()
    {
        _csvParserMock
            .Setup(x => x.ParseUkrPoshtaZipAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Parser explosion"));

        await _service.ParseZipFileFromWebAsync();

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Critical error in parsing orchestrator.")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);

        _geocodingMock.Verify(x => x.GetCoordinatesAsync(It.IsAny<string>()), Times.Never);
        _toponymDataMock.Verify(x => x.RefreshToponymsInDbAsync(It.IsAny<List<Toponym>>()), Times.Never);
    }
}