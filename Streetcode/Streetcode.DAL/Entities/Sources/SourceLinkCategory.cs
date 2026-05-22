using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Specifications.Shared;

namespace Streetcode.DAL.Entities.Sources;

public class SourceLinkCategory : IHasStreetcodes
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public int ImageId { get; set; }
    public Image? Image { get; set; }
    public List<StreetcodeContent> Streetcodes { get; set; } = new ();
    public List<StreetcodeCategoryContent> StreetcodeCategoryContents { get; set; } = new ();
}
