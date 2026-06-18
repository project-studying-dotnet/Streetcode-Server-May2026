using Streetcode.BLL.DTO.Media.Art;
using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.BLL.DTO.Media.ArtSlides
{
    public class CreateStreetcodeArtSlideDto
    {
        public int Index { get; set; }
        public int TemplateId { get; set; }
        public int StreetcodeId { get; set; }

        public List<ArtSlideItemDto> ArtSlideItems { get; set; } = new();
    }
}
