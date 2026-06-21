using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Interfaces.Media.Images;
using Streetcode.DAL.Repositories.Realizations.Base;

namespace Streetcode.DAL.Repositories.Realizations.Media.Art
{
    public class StreetcodeArtSlideTemplateRepository : RepositoryBase<StreetcodeArtSlideTemplate>, IStreetcodeArtSlideTemplateRepository
    {
        public StreetcodeArtSlideTemplateRepository(StreetcodeDbContext dbContext)
            : base(dbContext)
        {
        }
    }
}