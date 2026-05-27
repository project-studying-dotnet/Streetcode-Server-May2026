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
        public static async Task FillSeedAsync(UserManager<User> userManager)
        {
            const string adminEmail = "admin@admin.com";
            const string adminPassword = "admin@1234";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new User
                {
                    Email = adminEmail,
                    UserName = adminEmail,
                    Name = "admin",
                    Surname = "adminovich",
                    Role = UserRole.MainAdministrator,
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(
                    newAdmin,
                    adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(
                        newAdmin,
                        UserRole.MainAdministrator.ToString());
                }
            }
        }
    }
}
