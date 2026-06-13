using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Streetcode.BLL.Interfaces.WebParsingUtils;

namespace Streetcode.BLL.Services.WebParsingUtils;

public class GeocodingService : IGeocoding
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeocodingService> _logger;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
    private const string NominatimBaseUrl = "https://nominatim.openstreetmap.org/search";

    public GeocodingService(HttpClient httpClient, ILogger<GeocodingService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        _retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r => r.StatusCode == System.Net.HttpStatusCode.TooManyRequests || !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    public async Task<(decimal Lat, decimal Lon)?> GetCoordinatesAsync(string address)
    {
        string url = $"{NominatimBaseUrl}?q={Uri.EscapeDataString(address)}&format=json&limit=1";
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "Streetcode-Admin-App-v1.0");

            var response = await _retryPolicy.ExecuteAsync(() => _httpClient.SendAsync(request));
            if (!response.IsSuccessStatusCode) return null;

            string jsonResponse = await response.Content.ReadAsStringAsync();
            return ParseJsonToCoordinateTuple(jsonResponse);
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Failed to geocode '{address}': {ex.Message}");
            return null;
        }
    }

    private (decimal Lat, decimal Lon)? ParseJsonToCoordinateTuple(string jsonResponse)
    {
        using var doc = JsonDocument.Parse(jsonResponse);
        var root = doc.RootElement;

        if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
        {
            var firstElement = root[0];
            if (firstElement.TryGetProperty("lat", out var latProp) && firstElement.TryGetProperty("lon", out var lonProp))
            {
                if (decimal.TryParse(latProp.GetString(), System.Globalization.CultureInfo.InvariantCulture, out decimal lat) &&
                    decimal.TryParse(lonProp.GetString(), System.Globalization.CultureInfo.InvariantCulture, out decimal lon))
                {
                    return (lat, lon);
                }
            }
        }
        return null;
    }
}