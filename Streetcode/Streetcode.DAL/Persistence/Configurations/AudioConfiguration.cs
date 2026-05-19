using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Media;

namespace Streetcode.DAL.Persistence.Configurations;

public class AudioConfiguration : IEntityTypeConfiguration<Audio>
{
    public void Configure(EntityTypeBuilder<Audio> builder)
    {
        builder.ToTable("audios", "media");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Title)
            .HasMaxLength(100);

        builder.Property(x => x.BlobName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.MimeType)
            .IsRequired()
            .HasMaxLength(10);

        builder.Ignore(x => x.Base64);
    }
}
