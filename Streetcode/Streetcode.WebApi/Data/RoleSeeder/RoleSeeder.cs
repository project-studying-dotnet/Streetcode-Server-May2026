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
                    var result = await roleManager.CreateAsync(
                        new IdentityRole<int>
                        {
                            Name = role
                        });

                    if (!result.Succeeded)
                    {
                        throw new InvalidOperationException(
                            $"Failed to create role '{role}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }
    }
}
