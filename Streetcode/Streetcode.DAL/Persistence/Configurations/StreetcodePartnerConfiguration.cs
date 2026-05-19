using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Partners;

namespace Streetcode.DAL.Persistence.Configurations;

public class StreetcodePartnerConfiguration : IEntityTypeConfiguration<StreetcodePartner>
{
    public void Configure(EntityTypeBuilder<StreetcodePartner> builder)
    {
        builder.Property(x => x.StreetcodeId)
            .IsRequired();

        builder.Property(x => x.PartnerId)
            .IsRequired();
    }
}
