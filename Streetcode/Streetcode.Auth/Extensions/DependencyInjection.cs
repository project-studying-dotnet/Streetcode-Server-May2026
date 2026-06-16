using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Streetcode.Auth.Data;
using Streetcode.Auth.Models.Entities;
using Streetcode.Auth.Services;
using Streetcode.Auth.Services.Interfaces.Logging;
using Streetcode.Auth.Services.Interfaces.Users;
using Streetcode.Auth.Services.Services.Logging;
using Streetcode.Auth.Services.Users;
using System.Text;

namespace Streetcode.Auth.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt").Get<Common.Configuration.JwtSettings>();

            if (jwtSettings == null)
            {
                throw new InvalidOperationException("JwtSettings section is missing in configuration.");
            }

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

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    ValidateIssuer = false,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero 
                };
            });

            return services;
        }
    }
}