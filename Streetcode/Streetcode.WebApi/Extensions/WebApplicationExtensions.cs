using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.Middleware;

namespace Streetcode.WebApi.Extensions;

[ExcludeFromCodeCoverage]
public static class WebApplicationExtensions
{
    public static async Task ApplyMigrations(this WebApplication app)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        try
        {
            logger.LogInformation("Checking for pending database migrations...");

            using (var scope = app.Services.CreateScope())
            {
                var streetcodeContext = scope.ServiceProvider.GetRequiredService<StreetcodeDbContext>();

                var pendingMigrations = (await streetcodeContext.Database.GetPendingMigrationsAsync()).ToList();

                if (pendingMigrations.Any())
                {
                    if (logger.IsEnabled(LogLevel.Information))
                    {
                        logger.LogInformation(
                            "Found {Count} pending migrations: {Migrations}", 
                            pendingMigrations.Count,
                            string.Join(", ", pendingMigrations));
                    }

                    await streetcodeContext.Database.MigrateAsync();
                    logger.LogInformation("Migrations applied successfully.");
                }
                else
                {
                    logger.LogInformation("Database is up to date, no migrations required.");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured during startup migration at startup.      ");
        }
    }

    public static void UseCustomMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<ExceptionMiddleware>();
    }
}
