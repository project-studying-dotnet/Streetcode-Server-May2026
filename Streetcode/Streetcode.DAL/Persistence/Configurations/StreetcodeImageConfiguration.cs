using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.DAL.Persistence.Configurations;

public class StreetcodeImageConfiguration : IEntityTypeConfiguration<StreetcodeImage>
{
    public void Configure(EntityTypeBuilder<StreetcodeImage> builder)
    {
        builder.Property(x => x.StreetcodeId)
            .IsRequired();

        builder.Property(x => x.ImageId)
            .IsRequired();
    }
}
