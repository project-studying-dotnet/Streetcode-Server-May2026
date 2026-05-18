using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Transactions;

namespace Streetcode.DAL.Persistence.Configurations;

public class TransactionLinkConfiguration : IEntityTypeConfiguration<TransactionLink>
{
    public void Configure(EntityTypeBuilder<TransactionLink> builder)
    {
        builder.ToTable("transaction_links", "transactions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.UrlTitle)
            .HasMaxLength(255);

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.StreetcodeId)
            .IsRequired();
    }
}
