using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Partners;
using Streetcode.DAL.Specifications.Base;

namespace Streetcode.DAL.Specifications.Partners;

public class PartnerByStreetcodeIdSpecification : BaseSpecification<Partner>
{
    public PartnerByStreetcodeIdSpecification(int streetcodeId)
        : base(p => p.Streetcodes.Any(sc => sc.Id == streetcodeId) || p.IsVisibleEverywhere)
    {
        Includes = p => p.Include(pl => pl.PartnerSourceLinks);
    }
}
