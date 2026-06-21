using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.BLL.DTO.Media.ArtSlides
{
    public class StreetcodeArtSlideDto
    {
        public int Id { get; set; }
        public int Index { get; set; }

        public int TemplateId { get; set; }
        public StreetcodeArtSlideTemplate Template { get; set; } = null!;

        public List<ArtSlideItemDto> ArtSlideItems { get; set; } = new();
        public int StreetcodeId { get; set; }
    }
}
