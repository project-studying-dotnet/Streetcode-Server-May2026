using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.DTO.Sources
{
    public class StreetcodeCategoryContentDTO
    {
        public string Text { get; set; }
        public int SourceLinkCategoryId { get; set; }
        public int StreetcodeId { get; set; }
    }
}
