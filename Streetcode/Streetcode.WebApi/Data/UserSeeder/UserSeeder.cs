using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Enums;

namespace Streetcode.WebApi.InitialData.UserSeeder
{
    [ExcludeFromCodeCoverage]
    public static class UserSeeder
    {
        public static async Task FillSeedAsync(UserManager<User> userManager, IConfiguration configuration)
        {
            var adminEmail = configuration["AdminSettings:Email"];
            var adminPassword = configuration["AdminSettings:Password"];

            if (string.IsNullOrWhiteSpace(adminEmail)
            || string.IsNullOrWhiteSpace(adminPassword))
            {
                throw new InvalidOperationException(
                    "Admin credentials are not configured.");
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    Name = "admin",
                    Surname = "adminovich",
                    Role = UserRole.MainAdministrator,
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(
                    adminUser,
                    adminPassword);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

            var isInRole = await userManager.IsInRoleAsync(
                adminUser,
                UserRole.MainAdministrator.ToString());

            if (!isInRole)
            {
                var roleResult = await userManager.AddToRoleAsync(
                    adminUser,
                    UserRole.MainAdministrator.ToString());

                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to assign admin role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
