    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Streetcode.BLL.DTO.Sources;

public class CategoryContentUpdateDTO
{
    public int SourceLinkCategoryId { get; set; }
    public int StreetcodeId { get; set; }
    public string Text { get; set; } = null!;
}
