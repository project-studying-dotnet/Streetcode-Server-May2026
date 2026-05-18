using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Timeline;

namespace Streetcode.DAL.Persistence.Configurations;

public class HistoricalContextConfiguration : IEntityTypeConfiguration<HistoricalContext>
{
    public void Configure(EntityTypeBuilder<HistoricalContext> builder)
    {
        builder.ToTable("historical_contexts", "timeline");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(50);
    }
}
