using System;
using System.Collections.Generic;
using System.Text;

namespace Streetcode.BLL.DTO.Streetcode.TextContent.Text
{
    public class TextUpdateDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string TextContent { get; set; } = string.Empty;
        public int StreetcodeId { get; set; }
        public string? AdditionalText { get; set; }
    }
}
