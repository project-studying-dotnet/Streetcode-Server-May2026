using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Streetcode.TextContent;

namespace Streetcode.DAL.Persistence.Configurations;

public class RelatedTermConfiguration : IEntityTypeConfiguration<RelatedTerm>
{
    public void Configure(EntityTypeBuilder<RelatedTerm> builder)
    {
        builder.ToTable("related_terms", "streetcode");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Word)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.TermId)
            .IsRequired();

        builder.HasOne(rt => rt.Term)
            .WithMany(t => t.RelatedTerms)
            .HasForeignKey(rt => rt.TermId);
    }
}
