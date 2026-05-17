using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Team;

namespace Streetcode.DAL.Persistence.Configurations;

public class TeamMemberLinkConfiguration : IEntityTypeConfiguration<TeamMemberLink>
{
    public void Configure(EntityTypeBuilder<TeamMemberLink> builder)
    {
        builder.ToTable("team_member_links", "team");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.LogoType)
            .IsRequired();

        builder.Property(x => x.TargetUrl)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.TeamMemberId)
            .IsRequired();
    }
}
