using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.AdditionalContent;

namespace Streetcode.DAL.Persistence.Configurations;

public class SubtitleConfiguration : IEntityTypeConfiguration<Subtitle>
{
    public void Configure(EntityTypeBuilder<Subtitle> builder)
    {
        builder.ToTable("subtitles", "add_content");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.SubtitleText)
            .HasMaxLength(500);

        builder.Property(x => x.StreetcodeId)
            .IsRequired();
    }
}
