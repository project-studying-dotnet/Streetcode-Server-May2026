using Streetcode.DAL.Entities.Streetcode;

namespace Streetcode.DAL.Specifications.Shared;

public interface IHasStreetcodes
{
    List<StreetcodeContent> Streetcodes { get; }
}
