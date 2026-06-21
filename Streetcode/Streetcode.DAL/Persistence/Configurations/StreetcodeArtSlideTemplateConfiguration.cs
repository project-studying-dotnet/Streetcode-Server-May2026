using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Media.Images;
using Streetcode.DAL.Enums;

namespace Streetcode.DAL.Persistence.Configurations
{
    public class StreetcodeArtSlideTemplateConfiguration : IEntityTypeConfiguration<Entities.Media.Images.StreetcodeArtSlideTemplate>
    {
        public void Configure(EntityTypeBuilder<Entities.Media.Images.StreetcodeArtSlideTemplate> builder)
        {
            builder.ToTable("streetcode_art_slide_template", "media");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedNever();
            builder.Property(e => e.Name).IsRequired().HasMaxLength(100);

            builder.HasData(
                Enum.GetValues<ArtSlideTemplate>().Select(e => new StreetcodeArtSlideTemplate
                {
                    Id = (int)e,
                    Name = e.ToString()
                })
            );
        }
    }
}