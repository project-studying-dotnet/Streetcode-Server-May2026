using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Streetcode.BLL.Services.WebParsingUtils;
using Xunit;

namespace Streetcode.XUnitTest.BLL.Services.WebParsingUtils;

public class GeocodingServiceTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly Mock<ILogger<GeocodingService>> _loggerMock;

    public GeocodingServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _loggerMock = new Mock<ILogger<GeocodingService>>();
    }

    private GeocodingService CreateService(string baseUrl = "https://fake-geocode.org")
    {
        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        var settings = new GeocodingSettings { BaseUrl = baseUrl };
        var options = Options.Create(settings);

        return new GeocodingService(httpClient, _loggerMock.Object, options);
    }

    [Fact]
    public async Task GetCoordinatesAsync_WithValidResponse_ReturnsCorrectCoordinates()
    {
        var service = CreateService();
        var jsonResponse = "[{\"lat\":\"50.4501\",\"lon\":\"30.5234\"}]";

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        var result = await service.GetCoordinatesAsync("Київ");

        Assert.NotNull(result);
        Assert.Equal(50.4501m, result.Value.Lat);
        Assert.Equal(30.5234m, result.Value.Lon);
    }

    [Fact]
    public async Task GetCoordinatesAsync_WhenUrlIsNotConfigured_ThrowsInvalidOperationException()
    {
        var service = CreateService(baseUrl: "");

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.GetCoordinatesAsync("Львів"));
    }

    [Fact]
    public async Task GetCoordinatesAsync_WithEmptyJsonArray_ReturnsNull()
    {
        var service = CreateService();
        var jsonResponse = "[]";

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        var result = await service.GetCoordinatesAsync("Невідоме Місце");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCoordinatesAsync_WithInvalidJsonProperties_ReturnsNull()
    {
        var service = CreateService();
        var jsonResponse = "[{\"latitude\":\"50.4501\",\"longitude\":\"30.5234\"}]";

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        var result = await service.GetCoordinatesAsync("Київ");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCoordinatesAsync_WhenHttpErrorOccurs_ReturnsNull()
    {
        var service = CreateService();

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            });

        var result = await service.GetCoordinatesAsync("Одеса");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCoordinatesAsync_WhenTooManyRequests_PollyRetriesRequest()
    {
        var service = CreateService();
        var jsonResponse = "[{\"lat\":\"49.8397\",\"lon\":\"24.0297\"}]";
        int callCount = 0;

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(() =>
            {
                callCount++;
                if (callCount == 1)
                {
                    return new HttpResponseMessage { StatusCode = HttpStatusCode.TooManyRequests };
                }
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                };
            });

        var result = await service.GetCoordinatesAsync("Львів");

        Assert.NotNull(result);
        Assert.Equal(49.8397m, result.Value.Lat);
        Assert.Equal(24.0297m, result.Value.Lon);
        Assert.Equal(2, callCount);
    }

    [Fact]
    public async Task GetCoordinatesAsync_WhenExceptionThrown_ReturnsNullAndLogsWarning()
    {
        var service = CreateService();

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network failure"));

        var result = await service.GetCoordinatesAsync("Харків");

        Assert.Null(result);
    }
}