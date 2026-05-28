using System.ComponentModel.DataAnnotations;
using Streetcode.BLL.DTO.Media.Images;

namespace Streetcode.BLL.DTO.Sources;

public class SourceLinkCategoryDTO
{
    public int Id { get; set; }

    [Required]
    [MaxLength(23)]
    public string Title { get; set; } = null!;

    [Required]
    public int ImageId { get; set; }

    public ImageDTO? Image { get; set; }
}