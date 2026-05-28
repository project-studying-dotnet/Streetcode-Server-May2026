using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Streetcode.DAL.Persistence;

namespace Streetcode.WebApi.InitialData.SeederExtensions
{
    [ExcludeFromCodeCoverage]
    public static class SeederExtensions
    {
        public static async Task SeedIfEmptyAsync<T>(
            this DbSet<T> dbSet,
            IEnumerable<T> entities,
            StreetcodeDbContext context)
            where T : class
        {
            if (await dbSet.AnyAsync())
            {
                return;
             }

            await dbSet.AddRangeAsync(entities);

            await context.SaveChangesAsync();
        }
    }
}