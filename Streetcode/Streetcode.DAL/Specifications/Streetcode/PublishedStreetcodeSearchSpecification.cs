using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Specifications.Base;

namespace Streetcode.DAL.Specifications.Streetcode;

public class PublishedStreetcodeSearchSpecification : BaseSpecification<StreetcodeContent>
{
    public PublishedStreetcodeSearchSpecification(string searchQuery)
        : base(x => x.Status == StreetcodeStatus.Published &&
                    (x.Title!.Contains(searchQuery) ||
                     (x.Alias != null && x.Alias.Contains(searchQuery)) ||
                     x.Teaser!.Contains(searchQuery)))
    {
    }
}
