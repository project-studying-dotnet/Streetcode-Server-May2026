using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Streetcode;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Specifications.Base;

namespace Streetcode.DAL.Specifications.Streetcode;

public class StreetcodeForCatalogSpecification : BaseSpecification<StreetcodeContent>
{
    public StreetcodeForCatalogSpecification(int page, int count)
        : base(sc => sc.Status == StreetcodeStatus.Published)
    {
        Includes = src => src
            .Include(item => item.Tags)
            .Include(item => item.Images);

        Skip = (page - 1) * count;
        Take = count;
    }
}
