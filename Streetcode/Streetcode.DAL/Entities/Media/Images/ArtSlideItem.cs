namespace Streetcode.DAL.Entities.Media.Images
{
    public class ArtSlideItem
    {
        public int Id { get; set; }
        public int ArtId { get; set; }
        public int SlideId { get; set; }
        public int Index { get; set; }

        public Art Art { get; set; } = null!;
        public StreetcodeArtSlide Slide { get; set; } = null!;
    }
}
