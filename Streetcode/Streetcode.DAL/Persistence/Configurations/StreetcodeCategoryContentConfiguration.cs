using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Sources;

namespace Streetcode.DAL.Persistence.Configurations;

public class StreetcodeCategoryContentConfiguration : IEntityTypeConfiguration<StreetcodeCategoryContent>
{
    public void Configure(EntityTypeBuilder<StreetcodeCategoryContent> builder)
    {
        builder.Property(x => x.Text)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.SourceLinkCategoryId)
            .IsRequired();

        builder.Property(x => x.StreetcodeId)
            .IsRequired();
    }
}
