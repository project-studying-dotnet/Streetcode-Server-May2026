using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Toponyms;

namespace Streetcode.DAL.Persistence.Configurations;

public class ToponymConfiguration : IEntityTypeConfiguration<Toponym>
{
    public void Configure(EntityTypeBuilder<Toponym> builder)
    {
        builder.ToTable("toponyms", "toponyms");

        builder.Property(t => t.Id)
            .ValueGeneratedOnAdd();

        builder.Property(t => t.Oblast)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.AdminRegionOld)
            .HasMaxLength(150);

        builder.Property(t => t.AdminRegionNew)
            .HasMaxLength(150);

        builder.Property(t => t.Gromada)
            .HasMaxLength(150);

        builder.Property(t => t.Community)
            .HasMaxLength(150);

        builder.Property(t => t.StreetName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(t => t.StreetType)
            .HasMaxLength(50);

        builder.HasOne(d => d.Coordinate)
            .WithOne(p => p.Toponym)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
