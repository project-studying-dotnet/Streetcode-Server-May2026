using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using Streetcode.DAL.Enums;

namespace Streetcode.WebApi.Data.RoleSeeder
{
    [ExcludeFromCodeCoverage]
    public static class RoleSeeder
    {
        public static async Task FillSeedAsync(
            RoleManager<IdentityRole<int>> roleManager)
        {
            foreach (var role in Enum.GetNames<UserRole>())
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole<int>
                        {
                            Name = role
                        });
                }
            }
        }
    }
}
