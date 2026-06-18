using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.DAL.Persistence.Configurations
{
    public class StreetcodeArtSlideConfiguration : IEntityTypeConfiguration<StreetcodeArtSlide>
    {
        public void Configure(EntityTypeBuilder<StreetcodeArtSlide> builder)
        {
            builder.ToTable("streetcode_art_slide", "media");

            builder.HasKey(s => s.Id);

            builder.HasOne(s => s.Template)
                .WithMany()
                .HasForeignKey(s => s.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne<Streetcode.DAL.Entities.Streetcode.StreetcodeContent>()
                .WithMany()
                .HasForeignKey(s => s.StreetcodeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(s => s.Index).IsRequired();
        }
    }
}