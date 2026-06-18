using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Streetcode.BLL.Interfaces.BlobStorage;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.BLL.Interfaces.Instagram;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.Interfaces.Payment;
using Streetcode.BLL.Interfaces.Text;
using Streetcode.BLL.Interfaces.Users;
using Streetcode.BLL.Resources;
using Streetcode.BLL.Services.BlobStorageService;
using Streetcode.BLL.Services.Email;
using Streetcode.BLL.Services.Instagram;
using Streetcode.BLL.Services.Logging;
using Streetcode.BLL.Services.Payment;
using Streetcode.BLL.Services.Text;
using Streetcode.BLL.Services.Users;
using Streetcode.BLL.Settings;
using Streetcode.DAL.Entities.AdditionalContent.Email;
using Streetcode.DAL.Entities.Users;
using Streetcode.DAL.Persistence;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.DAL.Repositories.Realizations.Base;

namespace Streetcode.WebApi.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static void AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
    }

    public static void AddCustomServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRepositoryServices();
        services.AddFeatureManagement();

        var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        services.AddAutoMapper(currentAssemblies);
        services.AddMediatR(currentAssemblies);

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(Streetcode.BLL.MediatR.Behaviors.ValidationBehavior<,>));

        var useAzureBlobStorage = configuration.GetValue<bool>("UseAzureBlobStorage");

        if (useAzureBlobStorage)
        {
            services.Configure<AzureBlobEnvironmentVariables>(
                configuration.GetSection("AzureBlob"));

            services.AddScoped<IBlobService, AzureBlobService>();
        }
        else
        {
            services.AddScoped<IBlobService, BlobService>();
        }

        services.AddScoped<ILoggerService, LoggerService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailPublisher, RabbitMqEmailPublisher>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IInstagramService, InstagramService>();
        services.AddScoped<ITextService, AddTermsToTextService>();
    }

    public static void AddApplicationServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(ErrorMessages.DefaultConnectionIsMissing);

        var emailConfig = configuration
            .GetSection("EmailConfiguration")
            .Get<EmailConfiguration>()
            ?? throw new InvalidOperationException(ErrorMessages.EmailConfigurationIsMissing);

        services.AddSingleton(emailConfig);

        services.Configure<RabbitMqSettings>(
            configuration.GetSection("RabbitMq"));

        services.AddDbContext<StreetcodeDbContext>(options =>
        {
            options.UseSqlServer(connectionString, opt =>
            {
                opt.MigrationsAssembly(typeof(StreetcodeDbContext).Assembly.GetName().Name);
                opt.MigrationsHistoryTable("__EFMigrationsHistory", schema: "entity_framework");
            });
        });

        services.AddIdentity<User, IdentityRole<int>>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<StreetcodeDbContext>()
        .AddDefaultTokenProviders();

        services.AddHangfire(config =>
        {
            config.UseSqlServerStorage(connectionString);
        });

        var isHangfireEnabled = configuration.GetValue<bool>("Hangfire:Enabled");

        if (isHangfireEnabled)
        {
            services.AddHangfireServer();
        }

        services.AddCors(opt =>
        {
            opt.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        services.AddHsts(opt =>
        {
            opt.Preload = true;
            opt.IncludeSubDomains = true;
            opt.MaxAge = TimeSpan.FromDays(30);
        });

        services.AddLogging();
        services.AddControllers();

        services.AddAuthenticationServices(configuration);
    }

    public static void AddAuthenticationServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()
            ?? throw new InvalidOperationException("Jwt configuration section is missing.");

        services.AddSingleton(jwtSettings);
        services.AddScoped<ITokenService, TokenService>();
        services.AddAuthorization();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                RoleClaimType = ClaimTypes.Role,
            };
        });
    }

    public static void AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyApi", Version = "v1" });
            opt.CustomSchemaIds(x => x.FullName);

            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
            });

            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                    },
                    Array.Empty<string>()
                },
            });
        });
    }

    public class CorsConfiguration
    {
        public List<string> AllowedOrigins { get; set; } = new ();
        public List<string> AllowedHeaders { get; set; } = new ();
        public List<string> AllowedMethods { get; set; } = new ();
        public int PreflightMaxAge { get; set; }
    }
}