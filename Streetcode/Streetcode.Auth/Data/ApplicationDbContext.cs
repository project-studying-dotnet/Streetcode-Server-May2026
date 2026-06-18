using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Streetcode.Auth.Models.Entities;

namespace Streetcode.Auth.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {            
        }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("auth");

            builder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshTokens", "auth");
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.TokenHash).IsUnique();

                entity.Property(e => e.TokenHash).IsRequired();
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Created).IsRequired();
                entity.Property(e => e.Expires).IsRequired();
                entity.HasIndex(e => e.UserId);
            });

            builder.Entity<User>(entity =>
            {
                entity.Property(u => u.UserName).HasMaxLength(256);
            });

            builder.Entity<User>(entity => entity.ToTable("Users", "auth"));
            builder.Entity<IdentityRole<int>>(entity => entity.ToTable("Roles", "auth"));
            builder.Entity<IdentityUserRole<int>>(entity => entity.ToTable("UserRoles", "auth"));
            builder.Entity<IdentityUserClaim<int>>(entity => entity.ToTable("UserClaims", "auth"));
            builder.Entity<IdentityUserLogin<int>>(entity => entity.ToTable("UserLogins", "auth"));
            builder.Entity<IdentityRoleClaim<int>>(entity => entity.ToTable("RoleClaims", "auth"));
            builder.Entity<IdentityUserToken<int>>(entity => entity.ToTable("UserTokens", "auth"));
        }
    }
}