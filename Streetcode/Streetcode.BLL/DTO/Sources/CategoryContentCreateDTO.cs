using System.ComponentModel.DataAnnotations;

namespace Streetcode.BLL.DTO.Sources
{
    public class CategoryContentCreateDto
    {
        public int SourceLinkCategoryId { get; set; }
        public string Text { get; set; } = null!;
        public int StreetcodeId { get; set; }
    }
}