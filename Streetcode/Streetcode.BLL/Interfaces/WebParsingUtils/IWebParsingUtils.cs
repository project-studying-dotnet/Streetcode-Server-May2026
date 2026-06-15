using System;
using System.Collections.Generic;
using System.Text;

namespace Streetcode.BLL.Interfaces.WebParsingUtils
{
    public interface IWebParsingUtils
    {
        Task ParseZipFileFromWebAsync();
    }
}
