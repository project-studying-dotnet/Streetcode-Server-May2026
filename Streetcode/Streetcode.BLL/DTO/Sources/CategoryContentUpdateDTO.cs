    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Streetcode.BLL.DTO.Sources;

public class CategoryContentUpdateDTO
{
    [Required]
    public int SourceLinkCategoryId { get; set; }

    [Required]
    public int StreetcodeId { get; set; }

    [Required]
    [MaxLength(4000)]
    public string Text { get; set; } = null!;
}
