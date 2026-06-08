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

namespace Streetcode.Auth.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Измените "JwtSettings" на "Jwt"
            var jwtSettings = configuration.GetSection("Jwt").Get<Common.Configuration.JwtSettings>()
                              ?? throw new Exception("JwtSettings is missing in configuration!");
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

            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ILoggerService, LoggerService>();
            services.AddHostedService<TokenCleanupService>();

            return services;
        }
    }
}