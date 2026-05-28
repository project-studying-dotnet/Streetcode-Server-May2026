using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Enums;
using Streetcode.DAL.Persistence;

namespace Streetcode.WebApi.InitialData.UserSeeder
{
    [ExcludeFromCodeCoverage]
    public static class UserSeeder
    {
        public static async Task FillSeedAsync(StreetcodeDbContext dbContext)
        {
            const string AdminLiteral = "admin";
            var identityPasswordHasher = new PasswordHasher<User>();

            var adminUser = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == "admin");

            if (adminUser == null)
            {
                var newAdmin = new User
                {
                    Email = "admin@admin.com",
                    NormalizedEmail = "ADMIN@ADMIN.COM",
                    UserName = AdminLiteral,
                    NormalizedUserName = "ADMIN",
                    Name = AdminLiteral,
                    Surname = AdminLiteral,
                    Role = UserRole.MainAdministrator,
                    EmailConfirmed = true,
                };

                newAdmin.PasswordHash = identityPasswordHasher.HashPassword(newAdmin, AdminLiteral);

                await dbContext.Users.AddAsync(newAdmin);
                await dbContext.SaveChangesAsync();
            }
            else if (string.IsNullOrEmpty(adminUser.PasswordHash) || adminUser.PasswordHash.Length < 20)
            {
                adminUser.PasswordHash = identityPasswordHasher.HashPassword(adminUser, AdminLiteral);

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
