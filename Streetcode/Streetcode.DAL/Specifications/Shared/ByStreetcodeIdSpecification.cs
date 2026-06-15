using Microsoft.EntityFrameworkCore.Query;
using Streetcode.DAL.Specifications.Base;

namespace Streetcode.DAL.Specifications.Shared;

public class ByStreetcodeIdSpecification<T> : BaseSpecification<T>
    where T : class, IHasStreetcodes
{
    public ByStreetcodeIdSpecification(int streetcodeId)
        : base(entity => entity.Streetcodes.Any(s => s.Id == streetcodeId))
    {
    }

    public ByStreetcodeIdSpecification(
        int streetcodeId,
        Func<IQueryable<T>, IIncludableQueryable<T, object>> includes)
        : base(entity => entity.Streetcodes.Any(s => s.Id == streetcodeId))
    {
        Includes = includes;
    }
}
