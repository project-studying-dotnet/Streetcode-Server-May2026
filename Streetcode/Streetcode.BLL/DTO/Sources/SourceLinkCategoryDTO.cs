using System.ComponentModel.DataAnnotations;
using Streetcode.BLL.DTO.Media.Images;

namespace Streetcode.BLL.DTO.Sources;

public class SourceLinkCategoryDTO
{
    public int Id { get; set; }

    [Required]
    [MaxLength(23)]
    public string Title { get; set; } = null!;

    [Range(1, int.MaxValue)]
    public int ImageId { get; set; }

    public ImageDTO? Image { get; set; }
}