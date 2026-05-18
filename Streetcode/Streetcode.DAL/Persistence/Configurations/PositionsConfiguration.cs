using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Team;

namespace Streetcode.DAL.Persistence.Configurations;

public class PositionsConfiguration : IEntityTypeConfiguration<Positions>
{
    public void Configure(EntityTypeBuilder<Positions> builder)
    {
        builder.ToTable("positions", "team");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Position)
            .IsRequired()
            .HasMaxLength(50);
    }
}
