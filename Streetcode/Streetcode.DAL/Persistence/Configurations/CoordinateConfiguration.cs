using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates;
using Streetcode.DAL.Entities.AdditionalContent.Coordinates.Types;

namespace Streetcode.DAL.Persistence.Configurations;

public class CoordinateConfiguration : IEntityTypeConfiguration<Coordinate>
{
    public void Configure(EntityTypeBuilder<Coordinate> builder)
    {
        builder.ToTable("coordinates", "add_content");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Latitude)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(x => x.Longtitude)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.HasDiscriminator<string>("CoordinateType")
            .HasValue<Coordinate>("coordinate_base")
            .HasValue<StreetcodeCoordinate>("coordinate_streetcode")
            .HasValue<ToponymCoordinate>("coordinate_toponym");
    }
}

public class StreetcodeCoordinateConfiguration : IEntityTypeConfiguration<StreetcodeCoordinate>
{
    public void Configure(EntityTypeBuilder<StreetcodeCoordinate> builder)
    {
        builder.Property(x => x.StreetcodeId)
            .IsRequired();
    }
}

public class ToponymCoordinateConfiguration : IEntityTypeConfiguration<ToponymCoordinate>
{
    public void Configure(EntityTypeBuilder<ToponymCoordinate> builder)
    {
        builder.Property(x => x.ToponymId)
            .IsRequired();
    }
}
