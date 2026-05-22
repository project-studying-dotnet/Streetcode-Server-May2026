using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Specifications.Base;

namespace Streetcode.DAL.Specifications.Media;

public class ArtsByStreetcodeIdSpecification : BaseSpecification<Art>
{
    public ArtsByStreetcodeIdSpecification(int streetcodeId)
        : base(sc => sc.StreetcodeArts.Any(s => s.StreetcodeId == streetcodeId))
    {
        Includes = scl => scl.Include(sc => sc.Image)!;
    }
}
