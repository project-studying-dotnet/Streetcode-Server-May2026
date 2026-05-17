using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Analytics;

namespace Streetcode.DAL.Persistence.Configurations;

public class StatisticRecordConfiguration : IEntityTypeConfiguration<StatisticRecord>
{
    public void Configure(EntityTypeBuilder<StatisticRecord> builder)
    {
        builder.ToTable("qr_coordinates", "coordinates");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Address)
            .HasMaxLength(150);

        builder.HasOne(x => x.StreetcodeCoordinate)
            .WithOne(x => x.StatisticRecord)
            .HasForeignKey<StatisticRecord>(x => x.StreetcodeCoordinateId);
    }
}
