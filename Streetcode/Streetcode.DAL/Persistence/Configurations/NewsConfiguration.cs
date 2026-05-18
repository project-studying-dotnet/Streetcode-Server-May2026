using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.News;

namespace Streetcode.DAL.Persistence.Configurations;

public class NewsConfiguration : IEntityTypeConfiguration<News>
{
    public void Configure(EntityTypeBuilder<News> builder)
    {
        builder.ToTable("news", "news");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Text)
            .IsRequired();

        builder.Property(x => x.URL)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.URL)
            .IsUnique();

        builder.Property(x => x.CreationDate)
            .IsRequired();

        builder.HasOne(x => x.Image)
            .WithOne(x => x.News)
            .HasForeignKey<News>(x => x.ImageId);
    }
}
