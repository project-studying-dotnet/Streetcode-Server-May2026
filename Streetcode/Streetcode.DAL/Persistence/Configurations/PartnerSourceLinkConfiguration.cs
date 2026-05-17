using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Partners;

namespace Streetcode.DAL.Persistence.Configurations;

public class PartnerSourceLinkConfiguration : IEntityTypeConfiguration<PartnerSourceLink>
{
    public void Configure(EntityTypeBuilder<PartnerSourceLink> builder)
    {
        builder.ToTable("partner_source_links", "partners");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.LogoType)
            .IsRequired();

        builder.Property(x => x.TargetUrl)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.PartnerId)
            .IsRequired();
    }
}
