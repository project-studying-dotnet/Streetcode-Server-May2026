using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Timeline;

namespace Streetcode.DAL.Persistence.Configurations;

public class TimelineItemConfiguration : IEntityTypeConfiguration<TimelineItem>
{
    public void Configure(EntityTypeBuilder<TimelineItem> builder)
    {
        builder.ToTable("timeline_items", "timeline");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Date)
            .IsRequired();

        builder.Property(x => x.DateViewPattern)
            .IsRequired();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(600);
    }
}
