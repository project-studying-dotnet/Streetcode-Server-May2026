using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Toponyms;

namespace Streetcode.DAL.Persistence.Configurations;

public class StreetcodeToponymConfiguration : IEntityTypeConfiguration<StreetcodeToponym>
{
    public void Configure(EntityTypeBuilder<StreetcodeToponym> builder)
    {
        builder.Property(x => x.StreetcodeId)
            .IsRequired();

        builder.Property(x => x.ToponymId)
            .IsRequired();
    }
}
