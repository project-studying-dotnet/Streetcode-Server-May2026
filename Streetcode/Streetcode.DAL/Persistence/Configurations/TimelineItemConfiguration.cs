using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.DAL.Persistence.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Streetcode.DAL.Persistence.Configurations;

public sealed class TimelineItemConfiguration : IEntityTypeConfiguration<TimelineItem>
{
    #region IEntityTypeConfiguration<TimelineItem>
    public void Configure(EntityTypeBuilder<TimelineItem> builder)
    {
        builder.ToTable("timeline_items", "timeline");
        builder.HasKey(ti => ti.Id);
        builder.Property(ti => ti.Id).ValueGeneratedOnAdd();
        builder.Property(ti => ti.Date).IsRequired();
        builder.Property(ti => ti.DateViewPattern).IsRequired();
        builder.Property(ti => ti.Title).IsRequired().HasMaxLength(TimelineItemConstants.TitleMaxLength);
        builder.Property(ti => ti.Description).HasMaxLength(TimelineItemConstants.DescriptionMaxLength);
        builder.HasOne(ti => ti.Streetcode).WithMany(sc => sc.TimelineItems).HasForeignKey(ti => ti.StreetcodeId).IsRequired(false);
    }
    #endregion
}