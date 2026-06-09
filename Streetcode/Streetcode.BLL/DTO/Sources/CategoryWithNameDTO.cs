using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streetcode.BLL.DTO.Sources
{
    public class CategoryWithNameDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
    }
}
