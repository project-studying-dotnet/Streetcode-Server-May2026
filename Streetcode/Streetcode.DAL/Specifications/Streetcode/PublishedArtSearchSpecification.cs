using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Specifications.Base;

namespace Streetcode.DAL.Specifications.Streetcode;

public class PublishedArtSearchSpecification : BaseSpecification<Art>
{
    public PublishedArtSearchSpecification()
        : base(x => x.StreetcodeArts.Any(
            art => art.Streetcode != null &&
                   art.Streetcode.Status == StreetcodeStatus.Published))
    {
        Includes = q => q
            .Include(x => x.StreetcodeArts)
                .ThenInclude(sa => sa.Streetcode!);
    }
}
