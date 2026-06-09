using Streetcode.DAL.Persistence;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.DAL.Repositories.Realizations.Base;
using Streetcode.DAL.Repositories.Interfaces.Timeline;

namespace Streetcode.DAL.Repositories.Realizations.Timeline;

public sealed class TimelineRepository(
    StreetcodeDbContext dbContext
) : RepositoryBase<TimelineItem>(dbContext), ITimelineRepository;