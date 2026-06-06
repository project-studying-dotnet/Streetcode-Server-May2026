using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Streetcode.Auth.Data;
using Streetcode.Auth.Models.Entities;
using Streetcode.Auth.Services;
using Streetcode.Auth.Services.Interfaces.Logging;
using Streetcode.Auth.Services.Interfaces.Users;
using Streetcode.Auth.Services.Services.Logging;
using Streetcode.Auth.Services.Users;
using Streetcode.Auth.Settings;

namespace Streetcode.Auth.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = new JwtSettings();
            configuration.GetSection("JwtSettings").Bind(jwtSettings);
            services.AddSingleton(jwtSettings);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sql => sql.MigrationsHistoryTable("__AuthMigrationsHistory", "auth")));

            services.AddIdentity<User, IdentityRole<int>>(options => {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 3;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = false; 
                options.Password.RequireUppercase = false;       
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ILoggerService, LoggerService>();
            services.AddHostedService<TokenCleanupService>();

            return services;
        }
    }
}