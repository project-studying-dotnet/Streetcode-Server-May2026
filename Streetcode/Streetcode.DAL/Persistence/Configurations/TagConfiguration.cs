using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.AdditionalContent;

namespace Streetcode.DAL.Persistence.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tags", "add_content");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasMany(t => t.Streetcodes)
            .WithMany(s => s.Tags)
            .UsingEntity<StreetcodeTagIndex>(
                sp => sp.HasOne(x => x.Streetcode).WithMany(x => x.StreetcodeTagIndices).HasForeignKey(x => x.StreetcodeId),
                sp => sp.HasOne(x => x.Tag).WithMany(x => x.StreetcodeTagIndices).HasForeignKey(x => x.TagId));
    }
}
