using System.Threading.Tasks;

namespace Streetcode.BLL.Interfaces.WebParsingUtils;

public interface IGeocoding
{
    Task<(decimal Lat, decimal Lon)?> GetCoordinatesAsync(string address);
}