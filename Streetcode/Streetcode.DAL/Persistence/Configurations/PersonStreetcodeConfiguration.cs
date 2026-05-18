using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Streetcode.Types;

namespace Streetcode.DAL.Persistence.Configurations;

public class PersonStreetcodeConfiguration : IEntityTypeConfiguration<PersonStreetcode>
{
    public void Configure(EntityTypeBuilder<PersonStreetcode> builder)
    {
        builder.Property(p => p.FirstName)
            .HasMaxLength(50);

        builder.Property(p => p.Rank)
            .HasMaxLength(50);

        builder.Property(p => p.LastName)
            .HasMaxLength(50);
    }
}
