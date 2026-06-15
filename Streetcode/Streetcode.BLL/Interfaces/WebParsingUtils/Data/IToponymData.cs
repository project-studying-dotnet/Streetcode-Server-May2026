using System.Collections.Generic;
using System.Threading.Tasks;
using Streetcode.DAL.Entities.Toponyms;

namespace Streetcode.BLL.Interfaces.WebParsingUtils;

public interface IToponymData
{
    Task RefreshToponymsInDbAsync(List<Toponym> validToponyms);
}