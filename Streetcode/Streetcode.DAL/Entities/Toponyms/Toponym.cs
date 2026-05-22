using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Specifications.Shared;

namespace Streetcode.DAL.Entities.Toponyms;

public class Toponym : IHasStreetcodes
{
    public int Id { get; set; }
    public string Oblast { get; set; }
    public string? AdminRegionOld { get; set; }
    public string? AdminRegionNew { get; set; }
    public string? Gromada { get; set; }
    public string? Community { get; set; }
    public string StreetName { get; set; }
    public string? StreetType { get; set; }
    public List<StreetcodeContent> Streetcodes { get; set; } = new ();
    public ToponymCoordinate Coordinate { get; set; }
}
