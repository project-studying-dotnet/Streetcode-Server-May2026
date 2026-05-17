using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Sources;

namespace Streetcode.DAL.Persistence.Configurations;

public class SourceLinkCategoryConfiguration : IEntityTypeConfiguration<SourceLinkCategory>
{
    public void Configure(EntityTypeBuilder<SourceLinkCategory> builder)
    {
        builder.ToTable("source_link_categories", "sources");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ImageId)
            .IsRequired();

        builder.HasMany(d => d.StreetcodeCategoryContents)
            .WithOne(p => p.SourceLinkCategory)
            .HasForeignKey(d => d.SourceLinkCategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
