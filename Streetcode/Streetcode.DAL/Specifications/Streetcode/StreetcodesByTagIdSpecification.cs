using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Specifications.Base;

namespace Streetcode.DAL.Specifications.Streetcode;

public class StreetcodesByTagIdSpecification : BaseSpecification<StreetcodeContent>
{
    public StreetcodesByTagIdSpecification(int tagId)
        : base(sc => sc.Status == StreetcodeStatus.Published &&
                     sc.Tags.Select(t => t.Id).Any(id => id == tagId))
    {
        Includes = scl => scl
            .Include(sc => sc.Images)
            .Include(sc => sc.Tags);
    }
}
