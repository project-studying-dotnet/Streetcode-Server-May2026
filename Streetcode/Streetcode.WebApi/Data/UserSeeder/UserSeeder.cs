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
        public static async Task FillSeedAsync(StreetcodeDbContext dbContext, IConfiguration configuration)
        {
        }
    }
}
