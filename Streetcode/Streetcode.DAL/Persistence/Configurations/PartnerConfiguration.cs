using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Partners;

namespace Streetcode.DAL.Persistence.Configurations;

public class PartnerConfiguration : IEntityTypeConfiguration<Partner>
{
    public void Configure(EntityTypeBuilder<Partner> builder)
    {
        builder.ToTable("partners", "partners");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.LogoId)
            .IsRequired();

        builder.Property(x => x.IsKeyPartner)
            .IsRequired()
            .HasDefaultValue("false");

        builder.Property(x => x.IsVisibleEverywhere)
            .IsRequired();

        builder.Property(x => x.TargetUrl)
            .HasMaxLength(255);

        builder.Property(x => x.UrlTitle)
            .HasMaxLength(255);

        builder.Property(x => x.Description)
            .HasMaxLength(600);

        builder.HasMany(d => d.PartnerSourceLinks)
            .WithOne(p => p.Partner)
            .HasForeignKey(d => d.PartnerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
