using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.DAL.Entities.AdditionalContent;

public class Tag
{
    public int Id { get; set; }
    required public string Title { get; set; }
    public ICollection<StreetcodeTagIndex> StreetcodeTagIndices { get; set; }
        = new List<StreetcodeTagIndex>();

    public ICollection<StreetcodeContent> Streetcodes { get; set; }
        = new List<StreetcodeContent>();
}
