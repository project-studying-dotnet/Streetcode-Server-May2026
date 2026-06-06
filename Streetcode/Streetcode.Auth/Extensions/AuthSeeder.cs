using Microsoft.AspNetCore.Identity;
using Streetcode.Auth.Models.Entities;
using Streetcode.Common.Enums;

namespace Streetcode.Auth.Extensions
{
    public static class AuthSeeder
    {
        public static async Task SeedAsync(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager, IConfiguration configuration)
        {
            foreach (var roleNameItem in Enum.GetNames(typeof(UserRole)))
            {
                if (!await roleManager.RoleExistsAsync(roleNameItem))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(roleNameItem));
                }
            }

            var adminEmail = configuration["AdminSettings:Email"];
            var adminPassword = configuration["AdminSettings:Password"];

            var existingAdmin = await userManager.FindByEmailAsync(adminEmail!);
            if (existingAdmin == null)
            {
                var adminUser = new User
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    Name = "admin",
                    Surname = "adminovich",
                    Role = UserRole.MainAdministrator,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword!);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create admin user: {errors}");
                }
                existingAdmin = adminUser;
            }

            var roleName = UserRole.MainAdministrator.ToString();
            if (!await userManager.IsInRoleAsync(existingAdmin, roleName))
            {
                await userManager.AddToRoleAsync(existingAdmin, roleName);
            }
        }
    }
}