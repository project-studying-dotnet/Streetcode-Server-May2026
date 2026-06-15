using Microsoft.Extensions.Logging;
using Streetcode.BLL.Interfaces.WebParsingUtils;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Entities.Toponyms;

namespace Streetcode.BLL.Services.WebParsingUtils;

public class WebParsingUtilsService : IWebParsingUtils
{
    private readonly ICsvAddressParser _csvParserService;
    private readonly IGeocoding _geocodingService;
    private readonly IToponymData _toponymDataService;
    private readonly ILogger<WebParsingUtilsService> _logger;

    public WebParsingUtilsService(
        ICsvAddressParser csvParserService,
        IGeocoding geocodingService,
        IToponymData toponymDataService,
        ILogger<WebParsingUtilsService> logger)
    {
        _csvParserService = csvParserService;
        _geocodingService = geocodingService;
        _toponymDataService = toponymDataService;
        _logger = logger;
    }

    public async Task ParseZipFileFromWebAsync()
    {
        _logger.LogInformation("Start parcing");
        string tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        try
        {
            Directory.CreateDirectory(tempDirectory);

            var addresses = await _csvParserService.ParseUkrPoshtaZipAsync(tempDirectory);

            var validToponyms = new List<Toponym>();
            foreach (var addr in addresses)
            {
                string fullQuery = $"{addr.Oblast} область, {addr.StreetName}";
                var coords = await _geocodingService.GetCoordinatesAsync(fullQuery);

                if (coords != null)
                {
                    validToponyms.Add(new Toponym
                    {
                        Oblast = addr.Oblast,
                        AdminRegionOld = addr.AdminRegionOld,
                        Gromada = addr.Gromada,
                        Community = addr.Community,
                        StreetName = addr.StreetName,
                        StreetType = addr.StreetType,
                        Coordinate = new ToponymCoordinate
                        {
                            Latitude = coords.Value.Lat,
                            Longtitude = coords.Value.Lon
                        }
                    });
                }

                await Task.Delay(1000);
            }

            if (validToponyms.Count > 0)
            {
                await _toponymDataService.RefreshToponymsInDbAsync(validToponyms);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error in parsing orchestrator.");
        }
        finally
        {
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
        }
    }
}