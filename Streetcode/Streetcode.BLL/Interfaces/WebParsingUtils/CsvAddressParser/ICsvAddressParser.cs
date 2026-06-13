using System.Collections.Generic;
using System.Threading.Tasks;

namespace Streetcode.BLL.Interfaces.WebParsingUtils;

public interface ICsvAddressParser
{
    Task<List<TmpAddressModel>> ParseUkrPoshtaZipAsync(string tempDirectory);
}

public class TmpAddressModel
{
    public string Oblast { get; set; } = null!;
    public string? AdminRegionOld { get; set; }
    public string? AdminRegionNew { get; set; }
    public string? Gromada { get; set; }
    public string? Community { get; set; }
    public string StreetName { get; set; } = null!;
    public string? StreetType { get; set; }
}