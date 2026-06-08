using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Streetcode.Auth.Data;
using Streetcode.Auth.Models.Entities;

namespace Streetcode.Auth.Extensions
{
    public static class SeedExtensions
    {
        public static async Task SeedDataAsync(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
            var config = services.GetRequiredService<IConfiguration>();

            var dbContext = services.GetRequiredService<ApplicationDbContext>();

            //var r = await dbContext.Database.EnsureCreatedAsync();
            //var r = await dbContext.Database.EnsureDeletedAsync();
            //await dbContext.Database.EnsureCreatedAsync();
            try
            {
                await dbContext.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                // Поставьте брейкпоинт здесь
                Console.WriteLine($"Ошибка миграции: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Внутренняя ошибка: {ex.InnerException.Message}");
                }
            }

            await AuthSeeder.SeedAsync(userManager, roleManager, config);
        }
    }
}
