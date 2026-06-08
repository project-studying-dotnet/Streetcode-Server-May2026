using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Timeline;
using Streetcode.DAL.Persistence.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Streetcode.DAL.Persistence.Configurations;

public sealed class HistoricalContextConfiguration : IEntityTypeConfiguration<HistoricalContext>
{
    #region IEntityTypeConfiguration<HistoricalContext>
    public void Configure(EntityTypeBuilder<HistoricalContext> builder)
    {
        builder.ToTable("historical_contexts", "timeline");
        builder.HasKey(hc => hc.Id);
        builder.Property(hc => hc.Id).ValueGeneratedOnAdd();
        builder.Property(hc => hc.Title).IsRequired().HasMaxLength(HistoricalContextConstants.TitleMaxLength);
    }
    #endregion
}