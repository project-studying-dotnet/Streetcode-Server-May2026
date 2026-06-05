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
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StreetcodeDbContext>();

            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("Database is up to date.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during startup migration.");
        }
    }

    public static void UseCustomMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<ExceptionMiddleware>();
    }
}