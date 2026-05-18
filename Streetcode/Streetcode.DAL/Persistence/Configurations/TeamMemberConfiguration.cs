using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Streetcode.DAL.Entities.Team;

namespace Streetcode.DAL.Persistence.Configurations;

public class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.ToTable("team_members", "team");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.LastName)
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.IsMain)
            .IsRequired();

        builder.Property(x => x.ImageId)
            .IsRequired();

        builder.HasOne(x => x.Image)
            .WithOne(x => x.TeamMember)
            .HasForeignKey<TeamMember>(x => x.ImageId);

        builder.HasMany(x => x.Positions)
            .WithMany(x => x.TeamMembers)
            .UsingEntity<TeamMemberPositions>(
                tp => tp.HasOne(x => x.Positions)
                    .WithMany()
                    .HasForeignKey(x => x.PositionsId),
                tp => tp.HasOne(x => x.TeamMember)
                    .WithMany()
                    .HasForeignKey(x => x.TeamMemberId),
                j =>
                {
                    j.ToTable("team_member_positions", "team");
                    j.HasKey(x => new { x.TeamMemberId, x.PositionsId });
                });

        builder.HasMany(x => x.TeamMemberLinks)
            .WithOne(x => x.TeamMember)
            .HasForeignKey(x => x.TeamMemberId);
    }
}
