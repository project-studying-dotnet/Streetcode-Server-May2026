using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Timeline;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Streetcode.DAL.Persistence.Configurations;

public sealed class HistoricalContextTimelineConfiguration : IEntityTypeConfiguration<HistoricalContextTimeline>
{
    #region IEntityTypeConfiguration<HistoricalContextTimeline>
    public void Configure(EntityTypeBuilder<HistoricalContextTimeline> builder)
    {
        builder.HasKey(hct => new { hct.TimelineId, hct.HistoricalContextId });
        builder.HasOne(hct => hct.Timeline).WithMany(ti => ti.HistoricalContextTimelines).HasForeignKey(hct => hct.TimelineId);
        builder.HasOne(hct => hct.HistoricalContext).WithMany(hc => hc.HistoricalContextTimelines).HasForeignKey(hct => hct.HistoricalContextId);
    }
    #endregion
}