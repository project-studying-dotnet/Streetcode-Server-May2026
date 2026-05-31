using System.ComponentModel.DataAnnotations;

namespace Streetcode.BLL.DTO.Sources
{
    public class CategoryContentCreateDTO
    {
        [Required]
        public int SourceLinkCategoryId { get; set; }

        [Required]
        [MaxLength(4000)]
        public string Text { get; set; } = null!;

        [Required]
        public int StreetcodeId { get; set; }
    }
}