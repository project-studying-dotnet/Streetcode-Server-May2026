using System;
using System.Collections.Generic;
using System.Text;

namespace Streetcode.BLL.DTO.Media.Video
{
    public class VideoCreateDto
    {
        ////public string Title { get; set; } = string.Empty;
        ////public string? Description { get; set; }
        public string? Url { get; set; }
        public int StreetcodeId { get; set; }
    }
}
