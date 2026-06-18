using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.DAL.Entities.Media.Images
{
    public class StreetcodeArtSlide
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public int StreetcodeId { get; set; }

        public int TemplateId { get; set; }
        public StreetcodeArtSlideTemplate Template { get; set; } = null!;
        public List<ArtSlideItem> ArtSlideItems { get; set; } = new();
    }
}
