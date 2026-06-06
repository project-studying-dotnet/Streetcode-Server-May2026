using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Streetcode.Auth.Data;
using Streetcode.Auth.Extensions;
using Streetcode.Auth.MediatR.Behaviors;
using Streetcode.Auth.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddAutoMapper(typeof(Program).Assembly);

//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddIdentity<User, IdentityRole<int>>()
//    .AddEntityFrameworkStores<ApplicationDbContext>() // Укажите ваш контекст базы данных
//    .AddDefaultTokenProviders();

//builder.Host.UseSerilog((ctx, services, loggerConfiguration) =>
//{
//    loggerConfiguration
//        .ReadFrom.Configuration(builder.Configuration)
//        .Enrich.FromLogContext();
//});

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
{
    //app.UseSwagger();
    app.MapOpenApi();
    app.UseSwaggerUI(c =>
    {
        // c.SwaggerEndpoint("/swagger/v1/swagger.json", "Streetcode API v1");
        c.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}
else
{
    app.UseHsts();
}

await app.SeedDataAsync();

app.UseCors(policy => policy.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod());
app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();
app.MapControllers();

app.Run();