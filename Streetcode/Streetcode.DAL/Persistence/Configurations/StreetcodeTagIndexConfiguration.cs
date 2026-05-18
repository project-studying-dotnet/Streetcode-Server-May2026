using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.AdditionalContent;

namespace Streetcode.DAL.Persistence.Configurations;

public class StreetcodeTagIndexConfiguration : IEntityTypeConfiguration<StreetcodeTagIndex>
{
    public void Configure(EntityTypeBuilder<StreetcodeTagIndex> builder)
    {
        builder.ToTable("streetcode_tag_index", "add_content");

        builder.HasKey(nameof(StreetcodeTagIndex.StreetcodeId), nameof(StreetcodeTagIndex.TagId));

        builder.Property(x => x.StreetcodeId)
            .IsRequired();

        builder.Property(x => x.TagId)
            .IsRequired();

        builder.Property(x => x.IsVisible)
            .IsRequired();

        builder.Property(x => x.Index)
            .IsRequired();
    }
}
