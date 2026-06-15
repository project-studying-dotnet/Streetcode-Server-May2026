using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Media.Images;

namespace Streetcode.DAL.Persistence.Configurations.Media.Images
{
    public class ArtSlideItemConfiguration : IEntityTypeConfiguration<ArtSlideItem>
    {
        public void Configure(EntityTypeBuilder<ArtSlideItem> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Art)
                .WithMany() 
                .HasForeignKey(x => x.ArtId);

            builder.HasOne(x => x.Slide)
                .WithMany(s => s.ArtSlideItems)
                .HasForeignKey(x => x.SlideId);
        }
    }
}