using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Specifications.Base;

namespace Streetcode.DAL.Specifications.Streetcode;

public class PublishedStreetcodesSpecification : BaseSpecification<StreetcodeContent>
{
    public PublishedStreetcodesSpecification()
        : base(sc => sc.Status == StreetcodeStatus.Published)
    {
    }
}
