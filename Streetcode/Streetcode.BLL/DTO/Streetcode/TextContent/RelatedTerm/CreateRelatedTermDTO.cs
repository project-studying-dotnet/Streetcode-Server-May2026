using System;
using System.Collections.Generic;
using System.Text;

namespace Streetcode.BLL.DTO.Streetcode.TextContent.RelatedTerm
{
    public class CreateRelatedTermDTO
    {
        required public string Word { get; set; }
        public int TermId { get; set; }
    }
}
