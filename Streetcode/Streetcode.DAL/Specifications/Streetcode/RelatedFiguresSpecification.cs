using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Specifications.Base;

namespace Streetcode.DAL.Specifications.Streetcode;

public class RelatedFiguresSpecification : BaseSpecification<StreetcodeContent>
{
    public RelatedFiguresSpecification(IEnumerable<int> relatedIds)
        : base(sc => relatedIds.Any(id => id == sc.Id) && sc.Status == StreetcodeStatus.Published)
    {
        Includes = scl => scl
            .Include(sc => sc.Images)
                .ThenInclude(img => img.ImageDetails)
            .Include(sc => sc.Tags);
    }
}
