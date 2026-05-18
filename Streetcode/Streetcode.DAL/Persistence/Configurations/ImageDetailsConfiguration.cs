using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.DAL.Persistence.Configurations;

public class ImageDetailsConfiguration : IEntityTypeConfiguration<ImageDetails>
{
    public void Configure(EntityTypeBuilder<ImageDetails> builder)
    {
        builder.ToTable("image_details", "media");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Title)
            .HasMaxLength(100);

        builder.Property(x => x.Alt)
            .HasMaxLength(300);

        builder.Property(x => x.ImageId)
            .IsRequired();
    }
}
